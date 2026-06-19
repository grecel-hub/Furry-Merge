using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalFactory : MonoBehaviour
{
    private static Dictionary<AnimalType, AnimalType> evolutionMap = new Dictionary<AnimalType, AnimalType>()
    {
        { AnimalType.mouse, AnimalType.rabbit },
        { AnimalType.rabbit, AnimalType.sheep },
        { AnimalType.sheep, AnimalType.dog },
        { AnimalType.dog, AnimalType.tiger },
        { AnimalType.tiger, AnimalType.loong }
    };

    private static Dictionary<AnimalType, GameObject> prefabMap = new Dictionary<AnimalType, GameObject>();

    public static void RegisterPrefab(AnimalType type, GameObject prefab)
    {
        if (!prefabMap.ContainsKey(type))
        {
            prefabMap[type] = prefab;
        }
    }

    //创建动物
    public static GameObject CreateAnimal(AnimalType type, Vector3 position, Quaternion rotation)
    {
        if (prefabMap.ContainsKey(type))
        {
            return GameObject.Instantiate(prefabMap[type], position, rotation);
        }
        Debug.LogError("AnimalFactory: prefab for " + type + " not registered!");
        return null;
    }

    //获取下一个进化等级
    public static AnimalType GetNextEvolution(AnimalType type)
    {
        if (evolutionMap.ContainsKey(type))
            return evolutionMap[type];

        return type; // 已经是最后一级
    }
}
