using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject[] goodItems, badItems;
    public List<GameObject> goodActiveItems, badActiveItems, allSquares, nextSquares;

    public int score, difficulty;
    public float spawnRate;
    public bool gameActive;
    private GameObject scanner;
    
    public TextMeshProUGUI scoreText;

    private Transform spawnpos;

    void Awake()
    {
        scanner = GameObject.Find("Scanner");
    }

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        UpdateScore(0);
        //catalog all squares
        foreach (GameObject square in GameObject.FindGameObjectsWithTag("Square"))
        {
            allSquares.Add(square);
        }
    }

    void Update()
    {

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
            int index = Random.Range(0, goodActiveItems.Count);
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
        //
    }

    //returns a valid square for the next fruit to fall on
    GameObject NextSquare()
    {
       //choose a square from the active ones (determined by scanner collider)
    //    foreach (GameObject square in allSquares)
    //    {
    //         if(square.GetComponent<Square>().available)
    //         nextSquares.Add(square);
    //    }

        //if somehow no valid squares, just pick one close enough to the scanner
    //    if(nextSquares.Count == 0)
    //     {
    //         foreach (GameObject square in allSquares)
    //         {
    //             if(Vector3.Distance(scanner.transform.position, square.transform.position) < 3.8f)
    //             nextSquares.Add(square);
    //         }
    //     }
       //randomly choose the next square to go to
        int index = Random.Range(0, nextSquares.Count);
        return nextSquares[index];
    }

}
