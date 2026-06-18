using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public struct Fruit_data
{
    public Fruittype type;
    public Sprite sprite;
    public float add_size;
    public int score;
    public System.Drawing.Color color;
}
[CreateAssetMenu(fileName = "Fruit_mode", menuName = "Fruit/Fruit_mode")]
public class Fruit_mode : ScriptableObject
{
    public List<Fruit_data> list_fruit = new List<Fruit_data>();
}
