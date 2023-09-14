using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject[] goodItems, badItems;
    public List<GameObject> goodActiveItems, badActiveItems;

    int score;
    private float spawnRate;
    public TextMeshProUGUI scoreText;


    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        UpdateScore(0);
        StartCoroutine(SpawnTarget());

    }

    IEnumerator SpawnTarget()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRate);
            int index = Random.Range(0, goodActiveItems.Count);
            Instantiate(goodActiveItems[index]);

        }
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
