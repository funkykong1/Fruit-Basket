using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Count();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Count()
    {
        for (int i = 700; i > 200; i -= 13)
        {
            print(i);
        }
        //print(j);
    }
}
