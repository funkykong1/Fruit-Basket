using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private GameManager gm;
    private Rigidbody rb;

    private float maxTorque = 10;
    public float xRange, zRange;
    private float ySpawnPos = 40;

    void Start()
    {
        xRange = 14;
        zRange = 6;
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();   
    
        rb = GetComponent<Rigidbody>();
        rb.AddTorque(RandomTorque(), RandomTorque(), RandomTorque(), ForceMode.Impulse);

        transform.position = RandomSpawnPos();
    }

    void FixedUpdate()
    {
        int layerMask = 1 << 6;
        RaycastHit hit;
        //items fall faster if plr under them
        Debug.DrawRay(transform.position, (Vector3.down*50), Color.red);
        if(Physics.Raycast(transform.position, (Vector3.down*50), out hit, Mathf.Infinity, layerMask))
        {
            rb.drag = 0;
            rb.mass = 2;
        }
        else
        {
            rb.drag = 2;
            rb.mass = 1;
        }
    }


    //If hits player
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(this.tag == "Good Item")
                gm.UpdateScore(1);
            else
            {
                //todo gamee over
                gm.UpdateScore(-1);
            }
            Destroy(gameObject);
        }
        else
        {
            //some destroy animation. maybe turn the item black 
        }
    }


    float RandomTorque() {
        return Random.Range(-maxTorque, maxTorque);
    }
    Vector3 RandomSpawnPos() {
        return new Vector3(Random.Range(-xRange, xRange), ySpawnPos, Random.Range(-zRange, zRange));
    }


}
