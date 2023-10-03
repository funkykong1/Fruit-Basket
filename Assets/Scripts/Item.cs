using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private GameManager gm;
    private Rigidbody rb;

    //how fast it should be allowed to rotate
    private float maxTorque = 10;
    //how high should the thing spawn
    public float ySpawnPos;

    private SphereCollider sphere;
    private Mesh mesh;
    Vector3[] vertices;
    Color32[] colors;
    Color32 black;
    [Header("Current Distance")]
    [SerializeField]private float dist;
    private MeshRenderer rend;
    bool running = false, falling = false;
    public GameObject audioThing;

    //init
    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        //either get mesh components from this or a child object
        mesh = (GetComponent<MeshFilter>().mesh) ? GetComponent<MeshFilter>().mesh: GetComponentInChildren<MeshFilter>().mesh;
        rend = (GetComponent<MeshRenderer>()) ? GetComponent<MeshRenderer>(): GetComponentInChildren<MeshRenderer>();

        //gamemanager and player hitbox
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();   
        sphere = GameObject.Find("Player").GetComponent<SphereCollider>();

        //fruits will not collide with eachother
        Physics.IgnoreLayerCollision(7,7, true);
    }

    void Start()
    {

        //Change y spawn position for each prefab
        ySpawnPos = 26;

        //get colors and vertices of mesh
        Vector3[] vertices = mesh.vertices;
        Color32[] colors = mesh.colors32;
        black = new Color32(1,1,1,1);


        //add rotation to object, remove gravity and move it high, add mass too
        rb.AddTorque(RandomTorque(), RandomTorque(), RandomTorque(), ForceMode.Impulse);
        rb.angularDrag = 0.001f;
        rb.drag = 0.001f;
        rb.mass = 20;
        rb.useGravity = false;
        transform.position = new Vector3(transform.position.x, ySpawnPos, transform.position.z);
    }

    void FixedUpdate()
    {
        dist = Vector3.Distance(transform.position, GameObject.Find("Player").transform.position);

        if(rb.useGravity && !falling)
            StartCoroutine(PushDown());
    }



    public void SpeedCoroutine()
    {
        if(!running)
            StartCoroutine(AdjustSpeed());
        running = true;
    }

    //slightly adjust falling drag to give player more time to dodge
    //only applies if the fruit is in a northern square
    private IEnumerator AdjustSpeed()
    {
        yield return new WaitUntil(() => dist < 11);
        rb.drag = 2f;
        yield return new WaitForSeconds(0.4f);
        rb.drag = 0.01f;
    }

    //make fruit fall faster for lesser waiting times
    private IEnumerator PushDown()
    {
        falling = true;
        for (int i = 0; i < 10; i++)
        {
            int j = 7;
            if (gameObject.CompareTag("Good Item"))
                j = 10;
            
            rb.AddForce(Vector3.down*j, ForceMode.Impulse);
        }
        yield return null;
    }

    private void PlaySound(int good)
    {
        //instantiate audio instance and tell it if fruit is good or bad
        GameObject at = audioThing;
        at.GetComponent<AudioThing>().number = good;
        Instantiate(at);
    }
    //If hits player
    void OnTriggerEnter(Collider other)
    {
        if(other == sphere && gm.gameActive)
        {
            if(this.tag == "Good Item")
            {
                gm.UpdateScore(1);
                PlaySound(1);
            }
            else
            {
                gm.EndGame("stood under a dangerous item!");
                PlaySound(0);
            }
            Destroy(gameObject);
        }
    }

    //ground collision
    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Ground"))
        {
            if(gm.gameActive)
            {
                if(gameObject.CompareTag("Good Item"))
                {
                    gm.EndGame("let food go to waste!");
                    PlaySound(0);        
                }
                    
                else
                {
                    gm.UpdateScore(1);
                    PlaySound(2);
                }
                    
            }
            Destroy(gameObject);
        }
            
    }

    //change mesh color 
    //todo fix this
    private IEnumerator Death()
    {
        int i = 0;
        while(colors[i].r != 1)
            for (i = 0; i < colors.Length; i++)
            {
                mesh.Clear(true);
                mesh.vertices = vertices;
                Color32.Lerp(colors[i], black, Time.deltaTime * 5);
            }
        yield return null;
    }

    //random rotation
    float RandomTorque() {
        return Random.Range(-maxTorque, maxTorque);
    }


}
