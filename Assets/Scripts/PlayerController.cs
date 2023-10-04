using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    //how smoothly player should go to the next square
    [Range(0.1f, 0.6f)] [SerializeField] private float movementSmoothing = .05f;

    //scheduled moving //is the next fruit falling? //Animator & sound coroutine control
    public bool moving, fruitFalling, walking;

    //how fast is rotation
    public float rotationSpeed, moveSpeed, distance;

    //which square should player go to
    public GameObject targetSquare;

    //where player should be rotating to
    public Quaternion targetRotation;
    Vector3 m_EulerAngleVelocity;

    //dir used by LookRotation //velocity used by SmoothDamp
    Vector3 dir, velocity;

    private GameManager gm;
    public int currentFruit;
    public Animator anim;

    [SerializeField]
    private AudioClip[] grass;
    private AudioSource source;
    private Coroutine coroutine;


    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        source = GetComponent<AudioSource>();
    }

    void Start()
    {
        m_EulerAngleVelocity = new Vector3(0,100,0);
        dir = Vector3.left;
        velocity = Vector3.zero;
    }

    void Update()
    {
        if(gm.gameActive)
            //handles rotation
            Rotation();
    }

    //wasd and arrow key rotation
    void Rotation()
    {
        //ignore input if already moving
        if(!moving)
        {
            if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                StartCoroutine(Move(Vector3.forward));
            }
                

            if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                StartCoroutine(Move(Vector3.left));
            }
                
                
            if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                StartCoroutine(Move(Vector3.back));           
            }
                
                
            if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                StartCoroutine(Move(Vector3.right));                
            }
        }  
    }

    IEnumerator Move(Vector3 dir)
    {
        coroutine = StartCoroutine(Walk(false));
        moving = true;
        //simple rotation manipulation
        targetRotation = Quaternion.LookRotation(dir, Vector3.up);

        //rotate towards target direction
        while(transform.rotation != targetRotation)
        {
            var step = rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation,targetRotation,step);
            yield return new WaitForFixedUpdate();
        }
        
        //if no square ahead, do not move and let player rotate
        if(targetSquare == null)
        {
            moving = false;
            yield break;
        }

        //drop fruits via player movement
        if(currentFruit < gm.activeItems.Count)
            StartCoroutine(DropFruit());


        //smaller sphere & box colliders
        GetComponent<SphereCollider>().radius = 0.3f;
        this.GetComponent<BoxCollider>().enabled = false;

        //avoid editing the player's y value
        Vector3 mSquare = new Vector3(targetSquare.transform.position.x, transform.position.y, targetSquare.transform.position.z);
        //Only account for x and z axis for distance calculation
        Vector3 mPlayer = new Vector3(transform.position.x, targetSquare.transform.position.y, transform.position.z);

        //if remaining distance sufficiently small, stop
        distance = Vector3.Distance(transform.position, mSquare);

        while(distance > 0.05f)
        {
            mPlayer = new Vector3(transform.position.x, targetSquare.transform.position.y, transform.position.z);
            distance = Vector3.Distance(transform.position, mSquare);
            transform.position = Vector3.SmoothDamp(transform.position, mSquare, ref velocity, movementSmoothing, moveSpeed);
            yield return new WaitForFixedUpdate();
        }
        this.GetComponent<BoxCollider>().enabled = true;
        GetComponent<SphereCollider>().radius = 0.7f;
        yield return moving = false;
    }

    private IEnumerator DropFruit()
    {
        // fetch next fruit
        GameObject fruit = null;

        fruit = gm.activeItems[currentFruit];
        // tick up fruit counter
        currentFruit++;
        fruit.GetComponent<Rigidbody>().useGravity = true;

        //wait until player is comfortably in the square
        yield return new WaitUntil(() => !moving);

        //Check if the fruit still exists
        if(fruit)
        //player moves faster if a bad item is coming
            if(fruit.CompareTag("Bad Item"))
                StartCoroutine(AdjustSpeed());
        
    }

    //makes player faster for a bit
    private IEnumerator AdjustSpeed()
    {
        //get normal speed values
        float r = rotationSpeed;
        float s = moveSpeed;
        //adjust speed of player when dodging a dangerous thing
        rotationSpeed = 550;
        moveSpeed = 36;
        
        //if player is still in the dangerous square, wait until they move
        if(!moving)
            yield return new WaitUntil(() => moving);

        //stop walk coroutine and start running one
        StopCoroutine(coroutine);
        StartCoroutine(Walk(true));

        //if player is moving, wait until they stop in the next square
        yield return new WaitUntil(() => !moving);

        //restore previous values
        moveSpeed = s;
        rotationSpeed = r;
    }

    //walking/running animation + sound
    private IEnumerator Walk(bool fast)
    {
        if(fast)
        {
            source.clip = grass[1];
            anim.SetTrigger("Run");
        }
        else
        {
            source.clip = grass[0];
            anim.SetTrigger("Walk");
        }
        source.Play();
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => !moving);
        source.Stop();
        anim.SetTrigger("Stand");
    }


    //detect square infront to move to
    void OnTriggerEnter(Collider other)
    {
        //if other thing isnt a square or if plr moving, ignore colliders
        if(!other.CompareTag("Square"))
            return;

        targetSquare = other.gameObject;
    }

    //targetSquare nulled if rotating away from one
    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Square") && transform.rotation != targetRotation)
            targetSquare = null;
    }

}
