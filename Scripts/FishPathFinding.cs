using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class FishPathFinding : MonoBehaviour
{
    private List<Vector3> points = new List<Vector3>();
    public GameObject parentOfPoints;

    public float timer_time = 25f;
    private float time_current;

    private Vector3 current_point;
    
    public float movement_force = 20f;
    
    private Random random = new Random();

    private void Start()
    {
        int number_of_points = parentOfPoints.transform.childCount;
        //Debug.Log(number_of_points + "num of points");
        
        int i = 0;
        while (i < number_of_points)
        {
            points.Add(parentOfPoints.transform.GetChild(i).position);
            //Debug.Log(points.Count + " test of length");
            i++;
        }
        newPathPoint();
        

    }

    private void Update()
    {
        if (time_current < timer_time)
        {
            time_current += Time.deltaTime;
        }
        else
        {
            time_current = 0;
            newPathPoint();
        }
        
        moveToPoint();
    }

    private void newPathPoint()
    {
        int point = random.Next(0, parentOfPoints.transform.childCount);
        //Debug.Log(point + " point num");
        current_point = points[point];
        //this.transform.position = current_point;
    }

    private void moveToPoint()
    {
        
        Vector3 unit_vector = Vector3.Normalize(-this.transform.position + current_point);
        this.GetComponent<Rigidbody2D>().AddForce(unit_vector*movement_force);
    }
}
