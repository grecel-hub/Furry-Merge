using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    [HideInInspector]public AudioSourcemanager audiosourcemanager;
    public void Set_audio(AudioSourcemanager op)
    {
        if (panal_main == null)
        {
            panal_main = transform.Find("panal_main").gameObject;
        }
        panal_volum = panal_main.transform.Find("panal_volum").gameObject;
        btn_backtomain = panal_volum.transform.Find("btn_back").GetComponent<Button>();
        audiosourcemanager = op;
    }
    private GameObject panal_set, panal_volum;private GameObject panal_main, panal_end;
    private TextMeshProUGUI txt_score;
    private Button btn_play, btn_exit, btn_volum, btn_backtomain;
    //end,start
    private event Action action_start, action_end;
    //replay,next_play
    public event Action action_replay1, action_nextplay1, action_replay2,action_nextplay2;
    private Button btn_replay, btn_next;
    // Start is called before the first frame update
    void Start()
    {
        panal_end = transform.Find("panal_end").gameObject;
        txt_score = panal_end.transform.Find("GameOver/txt").GetComponent<TextMeshProUGUI>();
        panal_main = transform.Find("panal_main").gameObject;
        panal_set = panal_main.transform.Find("panal_set").gameObject ;
        btn_play = panal_set.transform.Find("replay").GetComponent<Button>();
        btn_exit = panal_set.transform.Find("exit").GetComponent<Button>();
        btn_volum = panal_set.transform.Find("volum").GetComponent<Button>();
        btn_play.onClick.AddListener(() => { panal_set.SetActive(false);SceneManager.LoadScene(1); });
        btn_exit.onClick.AddListener(() => { Game_end(); Application.Quit(); });
        btn_volum.onClick.AddListener(() => { panal_set.SetActive(false);panal_volum.SetActive(true); });
        btn_backtomain.onClick.AddListener(() => { panal_volum.SetActive(false);
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                panal_set.SetActive(true);
            }
            else
            {
                //游戏中可以控制音频面板
            }
        });
        //game_end
        btn_replay = panal_end.transform.Find("panal_play/btn_replay").GetComponent<Button>();
        btn_next = panal_end.transform.Find("panal_play/btn_next").GetComponent<Button>();
        btn_replay.onClick.AddListener(() => {
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                action_replay1?.Invoke();panal_end.SetActive(false);
            }
            else if(SceneManager.GetActiveScene().buildIndex == 2)
            {
                action_replay2?.Invoke(); panal_end.SetActive(false);
            }
        });
        btn_next.onClick.AddListener(() => {
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                panal_end.SetActive(false);
                action_nextplay1?.Invoke(); 
            }
            else if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                action_nextplay2?.Invoke(); panal_end.SetActive(false);
            }
        });
        panal_end.SetActive(false);Game_start();
    }
    public void Show_end(int score)//激活游戏结束面板
    {
        panal_end.SetActive(true);
        txt_score.text = score.ToString();
    }
    public void Add_start(Action op)
    {
        action_start += op;
    }
    public void Add_end(Action op)
    {
        action_end += op;
    }
    public void Game_start()
    {
        action_start?.Invoke();
    }
    public void Game_end()
    {
        action_end?.Invoke();
    }
}
