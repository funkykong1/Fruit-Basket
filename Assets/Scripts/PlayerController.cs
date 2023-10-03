using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    //how smoothly player should go to the next square
    [Range(0.1f, 0.6f)] [SerializeField] private float movementSmoothing = .05f;

    //scheduled moving //is the next fruit falling?
    public bool moving, fruitFalling;

    //how fast is rotation
    public float rotationSpeed, moveSpeed;

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



    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Start is called before the first frame update
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

    void LateUpdate()
    {
        //if moving queued and not rotating
        if(moving && transform.rotation == targetRotation)
            Move();
    }

    //wasd and arrow key rotation
    void Rotation()
    {
        
        //ignore input if already moving
        if(!moving)
        {
            if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                dir = Vector3.forward;
                moving = true;
            }
                

            if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                dir = Vector3.left;
                moving = true;
            }
                
                
            if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                dir = Vector3.back;
                moving = true;
            }
                
                
            if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                dir = Vector3.right;
                moving = true;
            }
        }

        //simple rotation manipulation
        var step = rotationSpeed * Time.deltaTime;
        targetRotation = Quaternion.LookRotation(dir, Vector3.up);

        //rotate towards target direction
        if(transform.rotation != targetRotation)
            transform.rotation = Quaternion.RotateTowards(transform.rotation,targetRotation,step);
            
    }

    void Move()
    {
        //if no square ahead, do not move and let player rotate
        if(targetSquare == null)
        {
            moving = false;
            return;
        }

        else if(!fruitFalling)
        {
            //drop fruits via player movement
            if(gm.activeItems[currentFruit] != null)
                StartCoroutine(DropFruit());

            anim.SetTrigger("Walk");
        }


        //smaller sphere collider
        GetComponent<SphereCollider>().radius = 0.3f;
        this.GetComponent<BoxCollider>().enabled = false;

        //avoid editing the player's y value
        Vector3 mSquare = new Vector3(targetSquare.transform.position.x, transform.position.y, targetSquare.transform.position.z);
        //Only account for x and z axis for distance calculation
        Vector3 mPlayer = new Vector3(transform.position.x, targetSquare.transform.position.y, transform.position.z);

        //if remaining distance sufficiently small, stop
        var distance = Vector3.Distance(transform.position, mSquare);
        if(distance > 0.05f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, mSquare, ref velocity, movementSmoothing, moveSpeed);
        }
        else
        {
            moving = false;
            this.GetComponent<BoxCollider>().enabled = true;
            GetComponent<SphereCollider>().radius = 0.7f;
            anim.SetTrigger("Stand");
        }
               
    }

    private IEnumerator DropFruit()
    {
        //if no target square, stop coroutine
        if(!targetSquare)
            yield break;

        fruitFalling = true;
        // fetch next fruit
        GameObject fruit = null;

        fruit = gm.activeItems[currentFruit];
        // tick up fruit counter
        currentFruit++;
        fruit.GetComponent<Rigidbody>().useGravity = true;
        fruit.GetComponent<Rigidbody>().AddForce(Vector3.down*2, ForceMode.Force);

        //wait until player is comfortably in the square
        yield return new WaitUntil(() => !moving);

        //player moves faster if a bad item is coming
        if(fruit.CompareTag("Bad Item"))
        {
            StartCoroutine(AdjustSpeed());
        }
        fruitFalling = false;
            
        
    }

    //makes player faster for a bit
    private IEnumerator AdjustSpeed()
    {
        //get normal speed values
        float r = rotationSpeed;
        float s = moveSpeed;
        //adjust speed of player when dodging a dangerous thing
        rotationSpeed = 500;
        moveSpeed = 32;
        
        //if player is still in the dangerous square, wait until they move
        if(!moving)
            yield return new WaitUntil(() => moving);

        anim.SetTrigger("Run");

        //if player is moving, wait until they stop in the next square
        yield return new WaitUntil(() => !moving);

        //restore previous values
        moveSpeed = s;
        rotationSpeed = r;
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
        if(other.CompareTag("Square"))
            targetSquare = null;
    }

}
