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
    public float ySpawnPos = 25;

    void Start()
    {
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();   
    
        rb = GetComponent<Rigidbody>();
        rb.AddTorque(RandomTorque(), RandomTorque(), RandomTorque(), ForceMode.Impulse);
        //move the item really high at first
        transform.position = new Vector3(transform.position.x, ySpawnPos, transform.position.z);
    }

    void FixedUpdate()
    {
        int layerMask = 1 << 6;
        RaycastHit hit;
        //items only fall if player under them
        Debug.DrawRay(transform.position, (Vector3.down*50), Color.red);
        if(Physics.Raycast(transform.position, (Vector3.down*50), out hit, Mathf.Infinity, layerMask))
        {
            rb.useGravity = true;
        }
        else
        {
            rb.useGravity = false;
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
                //todo bad items?
                gm.UpdateScore(-1);
            }
            Destroy(gameObject);
        }
        else if(other.CompareTag("Ground"))
        {
            //some destroy animation. maybe turn the item black
            
        }
    }


    float RandomTorque() {
        return Random.Range(-maxTorque, maxTorque);
    }


}
