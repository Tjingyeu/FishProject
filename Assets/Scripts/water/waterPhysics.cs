using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterPhysics : MonoBehaviour
{
    private Rigidbody rb;
    private Transform tf;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tf = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (tf.transform.position.y >= 400)
        {
            rb.useGravity = true;
        }
        else { rb.useGravity = false;}
    }
}
