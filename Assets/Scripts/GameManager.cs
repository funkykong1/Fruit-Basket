using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject[] goodItems, badItems;
    public List<GameObject> goodActiveItems, badActiveItems;

    int score;
    public float spawnRate;
    public TextMeshProUGUI scoreText;

    private Transform spawnpos;


    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        UpdateScore(0);
    }

    public void StartGame()
    {
        AddItems(true, 4);
        StartCoroutine(SpawnTarget());
    }

    IEnumerator SpawnTarget()
    {
        while (goodActiveItems.Count > 0)
        {
            yield return new WaitForSeconds(spawnRate);
            int rand = Random.Range(0, 1);
            if(rand == 0)
            {
                int index = Random.Range(0, goodActiveItems.Count);
                Instantiate(goodActiveItems[index],spawnpos);
            }
            else
            {
                int index = Random.Range(0, badActiveItems.Count);
                Instantiate(badActiveItems[index],spawnpos);
            }

        }
    }

    public void ButtonAdd()
    {
        AddItems(true, 1);
    }
    public void ButtonRemove()
    {
        RemoveItems(true, 1);
    }

    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
    }

    public void AddItems(bool good, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if(good)
            {
                goodActiveItems.Add(goodItems[Random.Range(0, goodItems.Length)]);
            }
            else
            {
                badActiveItems.Add(badItems[Random.Range(0, badItems.Length)]);
            }
        }
    }

    public void RemoveItems(bool good, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if(good)
            {
                goodActiveItems.RemoveAt(Random.Range(0, goodActiveItems.Count));
            }
            else
            {
                badActiveItems.RemoveAt(Random.Range(0, badActiveItems.Count));
            }
        }
    }

}
