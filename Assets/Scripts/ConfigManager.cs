using UnityEngine;
using System.IO;

[System.Serializable]
public class GameConfig
{

}

public class ConfigManager : MonoBehaviour
{
    public static GameConfig config;

    void Awake()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "config.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            config = JsonUtility.FromJson<GameConfig>(json);
            Debug.Log("配置加载成功");
        }
        else
        {
            Debug.LogError("配置文件未找到: " + path);
        }
    }
}
