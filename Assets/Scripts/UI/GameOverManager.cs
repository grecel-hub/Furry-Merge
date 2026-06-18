using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentScore;
    [SerializeField] private TextMeshProUGUI endScore;

    private void Start()
    {
        endScore.text = currentScore.text;

        EffectManager.instance.Gam_end();
        SFXManager.instance.losePlay();

        Time.timeScale = 0f;
    }

    private void RestartGame()
    {
        Debug.Log("RestartGame!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ChangMode()
    {
        SceneManager.LoadScene(1);
    }
}
