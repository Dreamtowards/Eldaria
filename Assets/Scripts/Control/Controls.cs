using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        Vector3 vel = new Vector3();
        float spd = 3;

        if (Input.GetKey(KeyCode.Space))
            vel += Vector3.up * spd;
        if (Input.GetKeyDown(KeyCode.LeftShift))
            vel += Vector3.down * spd;
        if (Input.GetKeyDown(KeyCode.W))
            vel += Vector3.forward * spd;
        if (Input.GetKeyDown(KeyCode.S))
            vel += Vector3.back * spd;

        GetComponent<Rigidbody>().velocity += vel;

    }
}
