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


    public List<GameObject> activeItems;
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
    private GameObject nextButton;

    //rgb colors
    private Color bright = new Color32(255,237,197,255);
    //hsv values, shadow float, scale shadow with sat
    public float hue,sat,brt, sdw;

    void Awake()
    {
        lamp = GameObject.Find("Directional Light").GetComponent<Light>();
        scanner = GameObject.Find("Scanner");
        player = GameObject.Find("Player");
        nextButton = GameObject.Find("Next Button");
        nextButton.SetActive(false);

        //HSV light colors
        hue = 41;
        sat = 23;
        brt = 100;


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

    void Update()
    {
        bright = Color.HSVToRGB(hue/360,sat/100,brt/100, false);
        lamp.color = bright;

        //shadows fade away with saturation
        if(sat > 0)
            sdw = sat/23;
        else
            sdw = 0;

        lamp.shadowStrength = sdw;

    }

    public void StartGame()
    {
        //get pos of random square, place plr and scanner there
        GameObject rnd = allSquares[Random.Range(0, allSquares.Count)];
        player.transform.position = new Vector3(rnd.transform.position.x, player.transform.position.y, rnd.transform.position.z);
        scanner.transform.position = player.transform.position;

        StartCoroutine(SpawnTarget());
    }

    public void NextStage()
    {
        difficulty++;
        StartCoroutine(SpawnTarget());
        //if difficulty such and so, change to a bigger map?
        //add bad items to pool?
        if(difficulty >= 5)
        {

        }
    }

    IEnumerator SpawnTarget()
    {
        //reset square active status
        foreach (GameObject square in allSquares)
        {
            square.GetComponent<Square>().squareActive = false;
        }

        yield return new WaitForSeconds(0.5f);

        //drop 'difficulty' amount of fruits
        for (int i = 0; i < difficulty; i++)
        {
            //call for the next available square
            GameObject square = NextSquare();
            GameObject fruit;
            int index;
            //20% of items are bad if difficulty high enough
            if(difficulty > 4 && Random.Range(1,5) == 1)
            {
                //spawn a random BAD thing on a square
                index = Random.Range(0, badItems.Length);
                fruit = Instantiate(badItems[index], square.transform.position, Quaternion.identity);
            }
            else
            {
                //spawn a random good thing on a square
                index = Random.Range(0, goodItems.Length);
                fruit = Instantiate(goodItems[index], square.transform.position, Quaternion.identity);
            }
            activeItems.Add(fruit);

            //move scanner there and wait a bit
            scanner.transform.position = square.transform.position;
            yield return new WaitForSeconds(spawnRate);
        }
        yield return new WaitForSeconds(3+difficulty/2);
        StartCoroutine(LightsOut());
    
    }

    //Call update score coroutine from elsewhere
    public void UpdateScore(int scoreToAdd)
    {
        StartCoroutine(ScoreAdder(scoreToAdd));
    }

    //use coroutine for wait function, smoother gameplay
    private IEnumerator ScoreAdder(int scoreToAdd)
    {
        yield return new WaitForSeconds(0.2f);
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
        int itemsLeft = 0;

        for (int i = 0; i < activeItems.Count; i++)
        {
            if(activeItems[i] != null)
                itemsLeft++;
        }
        if(itemsLeft == 0 && gameActive)
            StartCoroutine(LightsOn());
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
        while(brt < 100)
            {
                brt++;
                    if(sat < 23)
                    {
                        sat++;
                    }
                    yield return new WaitForFixedUpdate();
                    yield return new WaitForFixedUpdate();
            }
        nextButton.SetActive(true);
        yield return gameActive = false;
    }
    public void EndGame(string reason)
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
