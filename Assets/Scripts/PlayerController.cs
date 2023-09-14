using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    float horizontalMove, verticalMove;

    [Header("Movement Adjust")]

	[Space]
    public float speed;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;

    [Space]

    Vector3 velocity = Vector3.zero;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        verticalMove = Input.GetAxisRaw("Vertical") * speed;
        horizontalMove = Input.GetAxisRaw("Horizontal") * speed;
        MoveHorizontal(horizontalMove * Time.fixedDeltaTime);
        MoveVertical(verticalMove * Time.fixedDeltaTime);
    }

    void MoveHorizontal(float move)
    {
        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector3(move * 10f, rb.velocity.y, rb.velocity.z);
        // And then smoothing it out and applying it to the character
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, m_MovementSmoothing);
    }

    void MoveVertical(float move)
    {
        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector3(rb.velocity.x, rb.velocity.y, move * 10f);
        // And then smoothing it out and applying it to the character
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, m_MovementSmoothing);
    }

    
}
