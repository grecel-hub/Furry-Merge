using TMPro;
using UnityEngine;

public class Uimanager : MonoBehaviour
{
    public static Uimanager Instance;
    private void Awake()
    {
        Instance = this;
    }
    private TextMeshProUGUI cur_txt;
    private TextMeshProUGUI total_txt;
    [HideInInspector]public  int score;
    public void End_game()
    {
        score = 0; Add_scores(0);
    }
    // Start is called before the first frame update
    void Start()
    {
        cur_txt = transform.Find("Top/Score/txt").GetComponent<TextMeshProUGUI>();
        total_txt = transform.Find("Top/HighScore/txt").GetComponent<TextMeshProUGUI>();
        score = 0;Add_scores(0);
    }
    public void Add_scores(int op)
    {
        score += op;
        cur_txt.text =op.ToString();
        total_txt.text = score.ToString();
    }
}
