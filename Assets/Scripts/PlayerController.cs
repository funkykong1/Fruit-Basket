using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    //how smoothly player should go to the next square
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;

    //scheduled moving 
    public bool moving;

    //how fast is rotation
    public float rotationSpeed = 150f;

    //which square should player go to
    public GameObject targetSquare;

    //where player should be rotating to
    public Quaternion targetRotation;
    Vector3 m_EulerAngleVelocity;

    //dir used by LookRotation //velocity used by SmoothDamp
    Vector3 dir, velocity;



    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_EulerAngleVelocity = new Vector3(0,100,0);
        dir = Vector3.back;
        velocity = Vector3.zero;
    }

    void Update()
    {
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

        this.GetComponent<BoxCollider>().enabled = false;
        //avoid editing the player's y value
        Vector3 mSquare = new Vector3(targetSquare.transform.position.x, transform.position.y, targetSquare.transform.position.z);
        //Only account for x and z axis for distance calculation
        Vector3 mPlayer = new Vector3(transform.position.x, targetSquare.transform.position.y, transform.position.z);


        //if remaining distance sufficiently small, stop
        var distance = Vector3.Distance(transform.position, mSquare);

        if(distance > 0.1f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, mSquare, ref velocity, movementSmoothing, 1);
        }
        else
        {
            moving = false;
            this.GetComponent<BoxCollider>().enabled = true;
        }
               
    }


    //detect square infront to move to
    void OnTriggerEnter(Collider other)
    {
        //if other thing isnt a square or if plr moving, ignore colliders
        if(!other.CompareTag("Square"))
            return;


        targetSquare = other.gameObject;
        Debug.Log(targetSquare.name+ " found");
        
    }

    //targetSquare nulled if rotating away from one
    void OnTriggerLeave(Collider other)
    {
        if(other.CompareTag("Square"))
            targetSquare = null;
    }




}
