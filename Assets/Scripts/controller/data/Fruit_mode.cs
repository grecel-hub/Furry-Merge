using UnityEngine;
using System;
using System.Collections.Generic;
[Serializable]
public enum Fruittype
{
    fruit1=1, fruit2=2, fruit3=3, fruit4=4, fruit5=5, fruit6=6
}
[Serializable]
public struct Fruit_data
{
    public Fruittype type;
    public Sprite sprite;
    public float add_size;
    public int score;
    public Color color;
}
[CreateAssetMenu(fileName = "Fruit_mode", menuName = "Fruit/Fruit_mode")]
public class Fruit_mode : ScriptableObject
{
    public List<Fruit_data> list_fruit = new List<Fruit_data>();
}
