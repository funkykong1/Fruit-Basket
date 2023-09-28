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
    private MeshRenderer rend;
    bool running = false;

    //init
    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        //either get mesh components from this or a child object
        mesh = (GetComponent<MeshFilter>().mesh) ? GetComponent<MeshFilter>().mesh: GetComponentInChildren<MeshFilter>().mesh;
        rend = (GetComponent<MeshRenderer>()) ? GetComponent<MeshRenderer>(): GetComponentInChildren<MeshRenderer>();

        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();   
        sphere = GameObject.Find("Player").GetComponent<SphereCollider>();
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
        if(dist < 11 && !running)
        {
            StartCoroutine(AdjustSpeed());
            running = true;
        }
    }

    //slightly adjust falling drag to give player more time to dodge
    private IEnumerator AdjustSpeed()
    {
        rb.drag = 0.65f;
        rb.mass = 18;
        yield return new WaitForSeconds(0.3f);
        rb.mass = 19;
        rb.drag = 0.01f;
    }

    private void PlaySound(bool good)
    {
        //references to necessary components
        AudioSource source;
        GameObject audio = new GameObject("TEMP AUDIO");
        AudioManager manager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();

        Destroy(audio, 3);

        Instantiate(audio, new Vector3(transform.position.x, ySpawnPos, transform.position.z), Quaternion.identity);
        source = audio.AddComponent<AudioSource>();

        if(good)
            source.clip = manager.goodClips[Random.Range(0,manager.goodClips.Length)];
        else
            source.clip = manager.badClips[Random.Range(0,manager.badClips.Length)];

        source.Play();
    }
    //If hits player
    void OnTriggerEnter(Collider other)
    {
        if(other == sphere && gm.gameActive)
        {
            if(this.tag == "Good Item")
            {
                gm.UpdateScore(1);
                PlaySound(true);
            }
            else
            {
                gm.EndGame("stood under a dangerous item!");
                PlaySound(false);
            }
            rend.enabled = false;

            //get collider in this item
            Collider col;
            if(GetComponent<Collider>())
                col = GetComponent<Collider>();
            else
                col = GetComponentInChildren<Collider>();


            col.enabled = false;

            Destroy(gameObject, 2);
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
                    gm.EndGame("let food go to waste!");
                else
                    gm.UpdateScore(1);
            }
            rb.mass = 3;
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
