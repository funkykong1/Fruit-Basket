using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{

    //is this square already being used in spawning?
    public bool squareActive;

    //is this square available as the next waypoint?
    public bool available;
    //is this square near the scanner?
    public bool nearby;

    private GameObject player;
    private GameObject scanner;

    //debug
    private LineRenderer lr;
    private GameManager gm;

    //vectors which ignore y-values
    Vector3 mPlayer, mScanner;
    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        player = GameObject.Find("Player");
        scanner = GameObject.Find("Scanner");
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }
    void Start()
    {
        lr.SetPosition(0, transform.position + Vector3.up);
        squareActive = false;
        available = false;
        nearby = false;

    }


    void Update()
    {
        // Player and scanner vectors, both ignore Y axis
        mPlayer = new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z);
        mScanner = new Vector3(scanner.transform.position.x, this.transform.position.y, scanner.transform.position.z);
        

        TrackPosition();
        
        //DEBUG
        //UpdateLines();
        lr.enabled = false;

        UpdateLists();
    }

    void TrackPosition()
    {

        // if player is ontop of this square, it is active
        if(Vector3.Distance(this.transform.position, mPlayer) <= 0.15f)
        {
            squareActive = true;
            available = false;
            nearby = false;
        }

        // if scanner is currently ontop of this, not available
        // also turn it active
        else if(Vector3.Distance(this.transform.position, mScanner )<= 1f)
        {
            squareActive = true;
            available = false;
            nearby = false;
        }
        // if the scanner is not ontop but still close enough, is available
        else if(Mathf.Abs(this.transform.position.x - mScanner.x) <= 2 && Vector3.Distance(transform.position, mScanner) <= 5)
        {
            nearby = true;
            if(!squareActive)
                available = true;
        }
            
        else if(Mathf.Abs(this.transform.position.z - mScanner.z) <= 2 && Vector3.Distance(transform.position, mScanner) <= 5)
        {
            nearby = true;
            if(!squareActive)
                available = true;
        }
        else
        {
            nearby = false;
            available = false;
        }
        
    }


    //add square to gamemanager list according to its position from scanner
    void UpdateLists()
    {
        //if square hasnt been used yet, use it
        if(!squareActive)
            if(available && !gm.nextSquares.Contains(this.gameObject))
                gm.nextSquares.Add(this.gameObject);
        
        //remove from valid listing if too far or already active
        if(!available || squareActive)
            if(gm.nextSquares.Contains(this.gameObject))
                gm.nextSquares.Remove(this.gameObject);
        
        //keep track of used squares aswell
        if(nearby && !gm.nearbySquares.Contains(this.gameObject))
            gm.nearbySquares.Add(this.gameObject);

        if(!nearby && gm.nearbySquares.Contains(this.gameObject))
            gm.nearbySquares.Remove(this.gameObject);
    }

    void UpdateLines()
    {
        if(available)
        {
            lr.enabled = true;
            lr.startColor = Color.green;
            lr.endColor = Color.cyan;
            lr.SetPosition(1, mScanner + Vector3.up);
        }
        else if(nearby)
        {
            lr.enabled = true;
            lr.startColor = Color.red;
            lr.endColor = Color.red;
            lr.SetPosition(1, mScanner + Vector3.up);
        }
        else
        {
            lr.enabled = false;
        }
    }


}
