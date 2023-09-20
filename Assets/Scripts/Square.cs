using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{

    //is this square already being used in spawning?
    public bool squareActive;

    //is this square available as the next waypoint?
    public bool available;

    private GameObject player;
    private GameObject scanner;
    private LineRenderer lr;
    private GameManager gm;
    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        player = GameObject.Find("Player");
        scanner = GameObject.Find("Scanner");
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }
    void Start()
    {
        squareActive = false;
        available = false;
        lr.SetPosition(0, transform.position);
    }


    void Update()
    {
        // Player and scanner vectors, both ignore Y axis
        Vector3 mPlayer = new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z);
        Vector3 mScanner = new Vector3(scanner.transform.position.x, this.transform.position.y, scanner.transform.position.z);
        
        // if player is ontop of this square, it is active
        if(Vector3.Distance(this.transform.position, mPlayer) <= 0.15f)
        {
            squareActive = true;
            available = false;
        }

        // if scanner is currently ontop of this, not available
        else if(Vector3.Distance(this.transform.position, mScanner )<= 1f)
        {
            available = false;
        }
        // if the scanner is not ontop but still close enough, is available
        else if(Mathf.Abs(this.transform.position.x - mScanner.x) <= 2 && Vector3.Distance(transform.position, mScanner) <= 5)
            available = true;
        else if(Mathf.Abs(this.transform.position.z - mScanner.z) <= 2 && Vector3.Distance(transform.position, mScanner) <= 5)
            available = true;
        else
            available = false;

        // if player is far away, make square available again        
        if(Vector3.Distance(this.transform.position, mPlayer) > 5f)
        {
            squareActive = false;
        }

        if(available)
        {
            lr.enabled = true;
            lr.SetPosition(1, mScanner);
        }
        else
        {
            lr.enabled = false;
        }
            UpdateLists();
    }

    void UpdateLists()
    {
        if(!squareActive)
        {
            if(available && !gm.nextSquares.Contains(this.gameObject))
                gm.nextSquares.Add(this.gameObject);

            else if(!available && gm.nextSquares.Contains(this.gameObject))
                gm.nextSquares.Remove(this.gameObject);
        }
        if(available && !gm.nearbySquares.Contains(this.gameObject))
            gm.nearbySquares.Add(this.gameObject);

        else if(!available && gm.nearbySquares.Contains(this.gameObject))
            gm.nearbySquares.Remove(this.gameObject);
    }


}
