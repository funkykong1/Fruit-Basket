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
    public List<GameObject> allSquares, badSquares;

    [Header("List of valid nearby squares")]
    public List<GameObject> nextSquares;

    public List<GameObject> nearbySquares;

    public int score;

    [Header("How many fruits to drop")]
    public int difficulty;

    [Header("Fruit spawn rate")]
    public float spawnRate;
    public bool gameActive, gameOver;
    private GameObject scanner, player;
    public TextMeshProUGUI scoreText;
    private Light lamp;
    private GameObject nextButton;

    //rgb colors
    private Color bright = new Color32(255,237,197,255);
    //hsv values, shadow float, scale shadow with sat
    public float hue,sat,brt, sdw;

    private Canvas start, loser;

    void Awake()
    {
        start = GameObject.Find("Menu Canvas").GetComponent<Canvas>();
        loser = GameObject.Find("Loser Canvas").GetComponent<Canvas>();

        lamp = GameObject.Find("Directional Light").GetComponent<Light>();

        scanner = GameObject.Find("Scanner");
        player = GameObject.Find("Player");
        nextButton = GameObject.Find("Next Button");

        nextButton.SetActive(false);

        loser.enabled = false;

        //HSV light colors
        hue = 41;
        sat = 23;
        brt = 100;

        DontDestroyOnLoad(this.gameObject);
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
        start.enabled = false;

        StartCoroutine(SpawnTarget());
    }

    public void NextStage()
    {
        //clear all lists and increase difficulty
        player.GetComponent<PlayerController>().currentFruit = 0;
        //destroy remaining items to avoid excess clutter
        for (int i = 0; i < activeItems.Count; i++)
        {
            if(activeItems[i] != null)
                Destroy(activeItems[i]);
        }
        activeItems.Clear();
        difficulty++;
        scanner.transform.position = player.transform.position;

        StartCoroutine(SpawnTarget());
    }


    IEnumerator SpawnTarget()
    {
        //doesnt work without this
        yield return new WaitForSeconds(0.1f);

        //reset square active status
        foreach (GameObject square in allSquares)
        {
            square.GetComponent<Square>().squareActive = false;
        }

        player.GetComponent<PlayerController>().moving = false;

        //drop 'difficulty' amount of fruits
        for (int i = 0; i < difficulty; i++)
        {
            //call for the next available square
            GameObject square = NextSquare();
            GameObject fruit;
            int index;

            int randomizer = 5;

            //25% of items bad if difficulty really high
            if(difficulty > 8)
                randomizer = 4;

            //20% of items are bad if difficulty high
            if(difficulty > 5 && Random.Range(1,randomizer) == 1)
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

            if(badSquares.Contains(square))
            {
                //invoke speed coroutine within fruit
                fruit.GetComponent<Item>().SpeedCoroutine();
            }


            //add fruit to the list
            activeItems.Add(fruit);

            //move scanner there and wait a bit
            scanner.transform.position = square.transform.position;
            yield return new WaitForSeconds(spawnRate);
        }
        yield return new WaitForSeconds(3+difficulty/3);
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
        gameActive = false;
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
        if(!gameOver)
            nextButton.SetActive(true);
    }

    public void EndGame(string reason)
    {
        if(!gameActive)
            return;
        gameActive = false;

        //drop remaining items
        StartCoroutine(DropAll());

        //turtle scared
        player.GetComponentInChildren<Animator>().SetTrigger("Lose");

        //enable gameover canvas
        loser.enabled = true;

        //display gameover text
        TextMeshProUGUI text = GameObject.Find("Reason Text").GetComponent<TextMeshProUGUI>();
        text.text = "you " + reason;


    }

    public void Restart()
    {
        StartCoroutine(isufmkjcd());
    }
    private IEnumerator isufmkjcd()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);


        //score reset
        gameOver = true;
        score = 0;
        UpdateScore(0);

        //difficulty lowered because nextstage adds to it
        difficulty--;
        loser.enabled = false;

        //wait until lights are on
        StartCoroutine(LightsOn());
        player.GetComponentInChildren<Animator>().SetTrigger("Stand");
        yield return new WaitUntil(() => brt == 100);
        yield return new WaitForSeconds(0.1f);
        //then spawn more fruits
        NextStage();
        gameOver = false;
    }

    IEnumerator DropAll()
    {
        for (int i = 0; i < activeItems.Count; i++)
        {
            if(activeItems[i] != null)
            {
                activeItems[i].GetComponent<Rigidbody>().useGravity = true;
                activeItems[i].GetComponent<Rigidbody>().mass = 30;
                yield return new WaitForSeconds(0.2f);
            }
        }
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
