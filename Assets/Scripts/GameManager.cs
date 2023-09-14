using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public List<GameObject> items;
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
        while (true) {
            yield return new WaitForSeconds(spawnRate);
            int index = Random.Range(0, items.Count);
            Instantiate(items[index]);

        }
    }

    private void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
    }
}
