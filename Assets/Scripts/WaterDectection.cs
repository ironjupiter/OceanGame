using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaterDectection : MonoBehaviour
{
    private bool in_water = false;
    private bool start_timer = false;
    private float time_to_restore = .25f;
    private float timer = 0f;

    private void Update()
    {
        if (!start_timer)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer > time_to_restore)
        {
            in_water = true;
            start_timer = false;
            timer = 0;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        start_timer = true;
    }
    
    void OnTriggerExit2D(Collider2D col)
    {
        in_water = false;
    }
    
    public bool getInWater()
    {
        return in_water;
    }
}
