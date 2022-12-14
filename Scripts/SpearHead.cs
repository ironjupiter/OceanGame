using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SpearHead : MonoBehaviour
{
    public int FishLayer = 3;
    private static int score = 0;
    public GameObject hit_particles;
    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("hit!");
        if (collision.gameObject.layer == FishLayer)
        {
            DistanceJoint2D connection = this.AddComponent<DistanceJoint2D>();
            connection.connectedBody = collision.gameObject.GetComponent<Rigidbody2D>();
            connection.distance = 0;

            collision.transform.GetComponent<FishPathFinding>().enabled = false;
            tempScoreChanger(100);

            GameObject particles = Instantiate(hit_particles);
            particles.transform.position = this.transform.position + new Vector3(0, 0, -.5f);
        }
    }

    public void tempScoreChanger(int add_score)
    {
        GameObject canvas = GameObject.Find("Canvas");
        score += add_score;
        canvas.transform.GetChild(0).GetComponent<TMP_Text>().text = "score: " + score;
    }
}
