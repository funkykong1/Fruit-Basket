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
    public float ySpawnPos = 10;

    private float decayTime = 5;
    private SphereCollider sphere;
    private Mesh mesh;
    Vector3[] vertices;
    Color32[] colors;
    Color32 black = new Color32(1,1,1,1);

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = mesh.vertices;
        Color32[] colors = mesh.colors32;
        Color32 black = new Color32(0,0,0,1);


        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();   

        sphere = GameObject.Find("Player").GetComponent<SphereCollider>();

        rb = GetComponent<Rigidbody>();
        rb.AddTorque(RandomTorque(), RandomTorque(), RandomTorque(), ForceMode.Impulse);
        //move the item really high at first
        transform.position = new Vector3(transform.position.x, ySpawnPos, transform.position.z);

        rb.mass = 3;
    }

    void FixedUpdate()
    {
        int layerMask = 1 << 6;
        RaycastHit hit;
        //items only fall if player under them
        Debug.DrawRay(transform.position, (Vector3.down*50), Color.red);
        if(Physics.Raycast(transform.position, (Vector3.down*50), out hit, Mathf.Infinity, layerMask))
        {
            if(hit.collider == sphere)
                rb.useGravity = true;
        }
        else
        {
            rb.useGravity = true;
        }
    }


    //If hits player
    void OnTriggerEnter(Collider other)
    {
        if(other == sphere)
        {
            if(this.tag == "Good Item")
                gm.UpdateScore(1);
            else
            {
                //todo bad items?
                gm.UpdateScore(-1);
            }
            Destroy(gameObject);
        }
        else if(other.CompareTag("Ground"))
        {
            //some destroy animation. maybe turn the item black
            StartCoroutine(Death());
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Ground"))
            this.StartCoroutine(Death());
    }

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

    float RandomTorque() {
        return Random.Range(-maxTorque, maxTorque);
    }


}
