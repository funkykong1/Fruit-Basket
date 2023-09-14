using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private GameManager gm;
    private Rigidbody rb;

    private float maxTorque = 10;
    public float xRange, zRange;
    private float ySpawnPos = 35;

    void Start()
    {
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();   
    
        rb = GetComponent<Rigidbody>();
        rb.AddTorque(RandomTorque(), RandomTorque(), RandomTorque(), ForceMode.Impulse);

        transform.position = RandomSpawnPos();
    }


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
        }

    }


    float RandomTorque() {
        return Random.Range(-maxTorque, maxTorque);
    }
    Vector3 RandomSpawnPos() {
        return new Vector3(Random.Range(-xRange, xRange), ySpawnPos, Random.Range(-zRange, zRange));
    }


}
