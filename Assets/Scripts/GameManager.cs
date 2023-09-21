using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Current items in item pool")]
    public GameObject[] goodItems;

    [Header("Current bad items in pool")]
    public GameObject[] badItems;


    public List<GameObject> goodActiveItems, badActiveItems;
    public List<GameObject> allSquares;

    [Header("List of valid nearby squares")]
    public List<GameObject> nextSquares;

    public List<GameObject> nearbySquares;

    public int score;

    [Header("How many fruits to drop")]
    public int difficulty;

    [Header("Fruit spawn rate")]
    public float spawnRate;

    public float fadeTime;
    public bool gameActive;
    private GameObject scanner, player;
    public TextMeshProUGUI scoreText;
    private Light lamp;

    //rgb colors
    private Color bright = new Color32(245,225,200,255);
    public float hue,sat,brt, sdw;

    void Awake()
    {
        lamp = GameObject.Find("Directional Light").GetComponent<Light>();
        scanner = GameObject.Find("Scanner");
        player = GameObject.Find("Player");

        //HSV light colors
        hue = 45;
        sat = 15;
        brt = 100;

    }

    void Update()
    {
        bright = Color.HSVToRGB(hue/360,sat/100,brt/100, false);
        lamp.color = bright;

        //shadows fade away with saturation
        if(sat > 0)
            sdw = sat/18;
        else
            sdw = 0;

        lamp.shadowStrength = sdw;

    }

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        UpdateScore(0);
        gameActive = false;
        lamp.color = bright;
        //catalog all squares
        foreach (GameObject square in GameObject.FindGameObjectsWithTag("Square"))
        {
            allSquares.Add(square);
        }
    }

    public void StartGame()
    {
        //get pos of random square, place plr and scanner there
        GameObject rnd = allSquares[Random.Range(0, allSquares.Count)];
        player.transform.position = new Vector3(rnd.transform.position.x, player.transform.position.y, rnd.transform.position.z);
        scanner.transform.position = player.transform.position;

        StartCoroutine(SpawnTarget());
    }

    IEnumerator SpawnTarget()
    {
        yield return new WaitForSeconds(2);

        //drop 'difficulty' amount of fruits
        for (int i = 0; i < difficulty; i++)
        {
            //call for the next available square
            GameObject square = NextSquare();
            //yield return new WaitUntil(() => NextSquare());

            //spawn a random thing on it
            int index = Random.Range(0, goodItems.Length);
            GameObject fruit = Instantiate(goodItems[index], square.transform.position, Quaternion.identity);
            goodActiveItems.Add(fruit);

            //move scanner there and wait a bit
            scanner.transform.position = square.transform.position;
            yield return new WaitForSeconds(spawnRate);
        }
        yield return new WaitForSeconds(3+difficulty/2);
        StartCoroutine(LightsOut());
    
    }

    IEnumerator LightsOut()
    {
        //adjust brightness here
        while(brt > 75)
            {
                brt--;
                if(sat > 0)
                {
                    sat--;
                }
                //wait for 2 frames to slow down weather change
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
            }
        yield return gameActive = true;
    }

    IEnumerator LightsOn()
    {
        while(brt < 96)
            {
                brt++;
                    if(sat < 18)
                    {
                        sat++;
                    }
                    yield return new WaitForFixedUpdate();
                    yield return new WaitForFixedUpdate();
            }
        StartGame();
        yield return gameActive = false;
    }

    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;

        if(goodActiveItems.Count == 0)
            StartCoroutine(LightsOn());
    }

    public void EndGame()
    {
        //TODO: make all remaining items fall
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
