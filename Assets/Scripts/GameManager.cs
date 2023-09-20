using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Current items in play")]
    public GameObject[] goodItems, badItems;
    public List<GameObject> goodActiveItems, badActiveItems, allSquares;

    [Header("List of valid nearby squares")]
    public List<GameObject> nextSquares;
    [Header("List of all nearby squares")]
    public List<GameObject> nearbySquares;

    public int score, difficulty;
    public float spawnRate;
    public bool gameActive;
    private GameObject scanner;
    
    public TextMeshProUGUI scoreText;

    void Awake()
    {
        scanner = GameObject.Find("Scanner");
    }

    // Start is called before the first frame update
    void Start()
    {
        scanner.transform.position = GameObject.Find("Player").transform.position;
        score = 0;
        UpdateScore(0);
        //catalog all squares
        foreach (GameObject square in GameObject.FindGameObjectsWithTag("Square"))
        {
            allSquares.Add(square);
        }
    }

    public void StartGame()
    {
        scanner.transform.position = GameObject.Find("Player").transform.position;
        StartCoroutine(SpawnTarget());
    }

    IEnumerator SpawnTarget()
    {
        for (int i = 0; i < difficulty; i++)
        {
            //call for the next available square
            GameObject square = NextSquare();
            //yield return new WaitUntil(() => NextSquare());

            //spawn a random thing on it
            int index = Random.Range(0, goodItems.Length);
            Instantiate(goodItems[index], square.transform.position, Quaternion.identity);

            //move scanner there and wait a bit
            scanner.transform.position = square.transform.position;
            yield return new WaitForSeconds(spawnRate);
        }
        gameActive = false;
    
    }

    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
    }

    public void EndGame()
    {
        //TODO: make all remaining items fall
        //restart button
    }

    //returns a valid square for the next fruit to fall on
    GameObject NextSquare()
    {
        int index;

        //randomly choose the next square to go to
        if(nextSquares.Count > 0)
        {
            index = Random.Range(0, nextSquares.Count);
            return nextSquares[index];
        }
        //if there arent any valid squares, just pick one nearby
        else
        {
            index = Random.Range(0, nearbySquares.Count);
            return nearbySquares[index];
        }
    }

}
