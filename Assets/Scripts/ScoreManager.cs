using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreUI;
    [SerializeField] private TextMeshProUGUI highestUI;

    private int score;
    private int highestScore;
    private string playerUUID;

    private string uuidFilePath;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

#if UNITY_WEBGL
        if (PlayerPrefs.HasKey("playerUUID"))
        {
            playerUUID = PlayerPrefs.GetString("playerUUID");
        }
        else
        {
            playerUUID = Guid.NewGuid().ToString();
            PlayerPrefs.SetString("playerUUID", playerUUID);
        }
#elif UNITY_ANDROID || UNITY_IOS
        uuidFilePath = Path.Combine(Application.persistentDataPath, "player_uuid.txt");

        if (File.Exists(uuidFilePath))
        {
            playerUUID = File.ReadAllText(uuidFilePath);
        }
        else
        {
            playerUUID = Guid.NewGuid().ToString();
            File.WriteAllText(uuidFilePath, playerUUID);
        }
#else
        // PC »ò Editor ²âÊÔÓÃ
        playerUUID = PlayerPrefs.GetString("playerUUID", Guid.NewGuid().ToString());
        PlayerPrefs.SetString("playerUUID", playerUUID);
#endif
    }

    private void Start()
    {
        score = 0;
        highestScore = PlayerPrefs.GetInt(playerUUID, 0);

        scoreUI.text = score.ToString();
        highestUI.text = highestScore.ToString();
    }

    public void AddScore(AnimalType type)
    {
        int level = (int)type;
        float value = 2 * Mathf.Pow(level, 2);
        score += (int)value;

        scoreUI.text = score.ToString();

        if (score > highestScore)
        {
            highestScore = score;
            highestUI.text = highestScore.ToString();
            PlayerPrefs.SetInt(playerUUID, highestScore);
            PlayerPrefs.Save();
        }
    }
}
