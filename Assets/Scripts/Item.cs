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
        ySpawnPos = 20;

        //get colors and vertices of mesh
        Vector3[] vertices = mesh.vertices;
        Color32[] colors = mesh.colors32;
        black = new Color32(1,1,1,1);


        //add rotation to object, remove gravity and move it high, add mass too
        rb.AddTorque(RandomTorque(), RandomTorque(), RandomTorque(), ForceMode.Impulse);
        rb.mass = 7;
        rb.drag = 0.2f;
        rb.useGravity = false;
        transform.position = new Vector3(transform.position.x, ySpawnPos, transform.position.z);
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
                //todo bad items?
                gm.badActiveItems.Remove(gameObject);
                gm.UpdateScore(-1);
            }
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Ground"))
        {
            if(gameObject.CompareTag("Good Item"))
                gm.EndGame();
            gm.badActiveItems?.Remove(gameObject);
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
