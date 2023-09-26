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

    private float decayTime = 5;
    private SphereCollider sphere;
    private Mesh mesh;
    Vector3[] vertices;
    Color32[] colors;
    Color32 black;
    [Header("Current Distance")]
    [SerializeField]private float dist;


    //init
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mesh = GetComponent<MeshFilter>().mesh;
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();   
        sphere = GameObject.Find("Player").GetComponent<SphereCollider>();
    }

    void Start()
    {

        //Change y spawn position for each prefab
        ySpawnPos = 35;

        //get colors and vertices of mesh
        Vector3[] vertices = mesh.vertices;
        Color32[] colors = mesh.colors32;
        black = new Color32(1,1,1,1);


        //add rotation to object, remove gravity and move it high, add mass too
        rb.AddTorque(RandomTorque(), RandomTorque(), RandomTorque(), ForceMode.Impulse);
        rb.angularDrag = 0.001f;
        rb.useGravity = false;
        transform.position = new Vector3(transform.position.x, ySpawnPos, transform.position.z);
    }

    void FixedUpdate()
    {
        //Use distance to adjust mass, item falls slower when higher up
        dist = Vector3.Distance(GameObject.Find("Player").transform.position, this.transform.position);
        if(dist < 20)
        {
            rb.mass = 4;
        }
        else
        {
            rb.mass = 13;
        }
        //add some drag too for more smoothness
        rb.drag = dist/100;
    }

    //If hits player
    void OnTriggerEnter(Collider other)
    {
        if(other == sphere)
        {
            if(this.tag == "Good Item")
            {
                gm.UpdateScore(1);
            }
            else
            {
                gm.EndGame("stood under a dangerous item!");
            }
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Ground"))
        {
            if(gameObject.CompareTag("Good Item"))
                gm.EndGame("let food go to waste!");
            else
                gm.UpdateScore(1);

            Destroy(gameObject,1.5f);
        }
            
    }

    //change mesh color 
    private IEnumerator Death()
    {
        int i = 0;
        while(colors[i].r != 1)
            for (i = 0; i < colors.Length; i++)
            {
                mesh.Clear(true);
                mesh.vertices = vertices;
                Color32.Lerp(colors[i], black, Time.deltaTime * decayTime);
            }
        yield return null;
    }

    //random rotation
    float RandomTorque() {
        return Random.Range(-maxTorque, maxTorque);
    }


}
