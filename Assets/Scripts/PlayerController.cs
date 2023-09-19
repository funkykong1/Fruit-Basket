using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;

    //scheduled moving
    public bool moving;
    public float rotationSpeed = 1.0f;
    public GameObject targetSquare;

    public Quaternion targetRotation;

    Vector3 m_EulerAngleVelocity;
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

        //if moving queued and not rotating
        if(moving && transform.rotation == targetRotation)
            Move();
        
    }

    //wasd and arrow key rotation
    void Rotation()
    {
        //simple rotation manipulation
        var step = rotationSpeed * Time.deltaTime;
        targetRotation = Quaternion.LookRotation(dir, Vector3.up);

        //rotate towards target direction
        if(transform.rotation != targetRotation)
            transform.rotation = Quaternion.RotateTowards(transform.rotation,targetRotation,step);

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
            
    }

    void Move()
    {
        //if no square ahead, do not move and let player rotate
        if(targetSquare == null)
            moving = false;

        if(!moving)
            return;
        


        var dir = transform.position - targetSquare.transform.position;
        var distance = Vector3.Distance(this.transform.position, targetSquare.transform.position);
        
        //avoid editing the y value
        Vector3 mPosition = new Vector3(transform.position.x,0,transform.position.y);
        Vector3 mSquare = new Vector3(targetSquare.transform.position.x,0,targetSquare.transform.position.y);

        if(distance > 0.1f)
            Vector3.SmoothDamp(mPosition, mSquare, ref velocity, movementSmoothing);
        else
        {
            moving = false;
        }

        
            
    }


    //detect square infront to move to
    void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Square"))
            return;

        targetSquare = other.gameObject;
        Debug.Log(targetSquare.name+ " found");
    }

    //targetSquare nulled if rotating away from one
    void OnTriggerLeave(Collider other)
    {
        if(other.CompareTag("Square") && transform.rotation != targetRotation)
            targetSquare = null;
    }




}
