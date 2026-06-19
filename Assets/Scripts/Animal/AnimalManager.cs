using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    public static AnimalManager instance { get; private set; }

    [Header("动物预制体")]
    public GameObject mouse;
    public GameObject rabbit;
    public GameObject sheep;
    public GameObject dog;
    public GameObject tiger;
    public GameObject loong;

    [Header("next动物预制体")]
    [SerializeField] private GameObject mouseNext;
    [SerializeField] private GameObject rabbitNext;
    [SerializeField] private GameObject sheepNext;

    public int nextAnimal = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        AnimalFactory.RegisterPrefab(AnimalType.mouse, mouse);
        AnimalFactory.RegisterPrefab(AnimalType.rabbit, rabbit);
        AnimalFactory.RegisterPrefab(AnimalType.sheep, sheep);
        AnimalFactory.RegisterPrefab(AnimalType.dog, dog);
        AnimalFactory.RegisterPrefab(AnimalType.tiger, tiger);
        AnimalFactory.RegisterPrefab(AnimalType.loong, loong);
    }

    public void SetNextAnimal()
    {
        nextAnimal = Random.Range(1, 4);

        if (nextAnimal == 1)
        {
            mouseNext.SetActive(true);
            rabbitNext.SetActive(false);
            sheepNext.SetActive(false);
        }

        else if (nextAnimal == 2)
        {
            mouseNext.SetActive(false);
            rabbitNext.SetActive(true);
            sheepNext.SetActive(false);
        }

        else
        {
            mouseNext.SetActive(false);
            rabbitNext.SetActive(false);
            sheepNext.SetActive(true);
        }
    }
}
