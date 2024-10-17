using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temp : MonoBehaviour
{
    public Rigidbody rigid;
    // Start is called before the first frame update
    void Start()
    {
        rigid.velocity = Vector3.up* 100;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rigid.velocity = Vector3.forward* 100;
        
    }
}
