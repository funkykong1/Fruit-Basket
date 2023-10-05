using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private Light lamp;
    [SerializeField]
    private GameObject[] maps;

    //rgb colors
    private Color bright;
    //hsv values, shadow float, scale shadow with sat
    public float hue,sat,brt, sdw;

    //dimmed values for saturation and brightness
    //only used as reference
    public float dSat, dBrt, increment;

    private UIManager ui;
    private bool hard;
    public int currentMap = 0;
    private Vector3 cameraVel;

    void Awake()
    {
        ui = GameObject.Find("UI Manager").GetComponent<UIManager>();
        lamp = GameObject.Find("Directional Light").GetComponent<Light>();

        scanner = GameObject.Find("Scanner");
        player = GameObject.Find("Player");


        //HSV light colors
        hue = 40;
        sat = 60;
        brt = 75;

        dSat = 0;
        dBrt = 45;

        currentMap = 0;
    }

    // Start is called before the first frame update
    void Start()
    {

        cameraVel = Vector3.zero;
        score = 0;
        UpdateScore(0);
        gameActive = false;
        lamp.color = bright;
        SquareThing();
    }


    void Update()
    {
        if(sat > 0)
            sdw = sat/60;
        else
            sdw = 0;
        
        bright = Color.HSVToRGB(hue/360,sat/100,brt/100, false);
        lamp.color = bright;
        lamp.shadowStrength = sdw;

    }

    public void DifficultySet(bool h)
    {
        hard = h;
    }

    public void StartGame()
    {
        //get pos of random square, place plr and scanner there
        GameObject rnd = allSquares[Random.Range(0, allSquares.Count)];
        player.transform.position = new Vector3(rnd.transform.position.x, player.transform.position.y, rnd.transform.position.z);
        scanner.transform.position = player.transform.position;
        ui.adjust.enabled = false;
        GameObject.Find("Cool Camera").SetActive(false);

        StartCoroutine(SpawnTarget());
    }

    public void NextStage()
    {
        if(difficulty >= 8 && currentMap == 0)
        {
            StartCoroutine(ChangeMap());
            return;
        }

        SquareThing();

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

    IEnumerator ChangeMap()
    {
        Destroy(maps[currentMap]);
        currentMap = 1;
        Instantiate(maps[currentMap]);

        yield return new WaitForSeconds(0.2f);
        SquareThing();

        Vector3 newCameraPos = new Vector3(0, 28, -13);
        GameObject camera = GameObject.Find("Main Camera");
        while(Vector3.Distance(camera.transform.position, newCameraPos) > 0.05f)
        {
            camera.transform.position = Vector3.SmoothDamp(camera.transform.position, newCameraPos, ref cameraVel, 0.2f, 2);
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitUntil(() => Vector3.Distance(camera.transform.position, newCameraPos) < 0.05f);
        NextStage();
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

        if(difficulty > 7)
            spawnRate = 0.4f;

        player.GetComponent<PlayerController>().moving = false;

        //drop 'difficulty' amount of fruits
        for (int i = 0; i < difficulty; i++)
        {
            //call for the next available square
            GameObject square = NextSquare();

            //if no square available, move on
            if(square == null)
                break;

                
            GameObject fruit;
            int index;

            int randomizer = 5;

            if(hard)
            {
                //25% of items bad if difficulty really high
                if(difficulty > 7)
                    randomizer = 4;

                //20% of items are bad if difficulty high
                if(difficulty > 4 && Random.Range(1,randomizer) == 1)
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
           
                    if(badSquares.Contains(square))
                    {
                        //invoke speed coroutine within fruit
                        fruit.GetComponent<Item>().SpeedCoroutine();
                    }
                }
            }

            //if player chooses chill mode, skip bad items altogether
            else
            {
                //spawn a random good thing on a square
                index = Random.Range(0, goodItems.Length);
                fruit = Instantiate(goodItems[index], square.transform.position, Quaternion.identity);
            }
            //add fruit to the list
            activeItems.Add(fruit);

            //move scanner there and wait a bit
            scanner.transform.position = square.transform.position;
            yield return new WaitForSeconds(spawnRate);
        }
        yield return new WaitForSeconds(2+difficulty/3);
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
        ui.scoreText.text = "Score: " + score;
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
        while(sat > dSat)
            {
                sat--;
                if(brt > dBrt)
                {
                    brt--;
                }
                //wait for a frame to slow down weather change
                yield return new WaitForFixedUpdate();
                //yield return new WaitForFixedUpdate();
            }
        yield return gameActive = true;
    }

    IEnumerator LightsOn()
    {
        
        gameActive = false;
        while(sat < 60)
            {
                sat++;
                    if(brt < 75)
                    {
                        brt++;
                    }
                    yield return new WaitForFixedUpdate();
                    //yield return new WaitForFixedUpdate();
            }
        if(!gameOver)
        {
            yield return new WaitForSeconds(1);
            NextStage();
        }
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
        ui.loser.enabled = true;

        //display gameover text
        ui.loseText.text = "you " + reason;

        //reset square active status
        foreach (GameObject square in allSquares)
        {
            square.GetComponent<Square>().squareActive = false;
        }


    }

    public void Menu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        ui.loser.enabled = false;

        //wait until lights are on
        StartCoroutine(LightsOn());
        player.GetComponentInChildren<Animator>().SetTrigger("Retry");
        yield return new WaitForSeconds(1.5f);
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
                yield return new WaitForSeconds(0.3f);
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
        else if(activeItems.Count <= 0)
        {
            index = Random.Range(0,nearbySquares.Count);
            return nearbySquares[index];
        }
        //if there arent any valid squares, just stop spawning stuff
        else
        {
            return null;
        }
    }

    //condensed square function
    void SquareThing()
    {
        //clear lists, squares add themselves to the lists if near scanner
        allSquares.Clear();
        nearbySquares.Clear();
        nextSquares.Clear();
        badSquares.Clear();

        //catalog all squares
        foreach (GameObject square in GameObject.FindGameObjectsWithTag("Square"))
        {
            allSquares.Add(square);
            //northern squares have a sphere collider, find and catalog them
            if(square.transform.position.z > 3)
                badSquares.Add(square);
        }
    }
}
