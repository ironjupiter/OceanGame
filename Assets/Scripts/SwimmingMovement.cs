using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwimmingMovement : MonoBehaviour
{
    private Transform this_transform;
    private Rigidbody2D rigidbody;
    public float impulse = 10;
    public float velocity;
    private float drag;
    private float mass;
    
    void Start()
    {
        this_transform = this.gameObject.transform;
        rigidbody = this.gameObject.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        addImpulse();
        
    }

    void addImpulse()
    {
        if (this.transform.GetChild(0).GetComponent<WaterDectection>().getInWater() == false)
        {
            Debug.Log("not in water");
            return;
        }
        
        if (rigidbody.velocity.magnitude > velocity && this.transform.GetChild(0).GetComponent<WaterDectection>().getInWater())
        {
            Debug.Log(rigidbody.velocity.magnitude);
            return;
        }
        
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            rigidbody.AddForce(new Vector2(impulse * Input.GetAxis("Horizontal"), impulse * Input.GetAxis("Vertical")));
        }

        impulse = 10;

    }

}
