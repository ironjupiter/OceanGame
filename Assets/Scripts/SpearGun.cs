using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class SpearGun : MonoBehaviour
{
    public GameObject cam;
    public float kickback = 2000;
    private bool isFired = false;
    public float fire_rate = 1;
    private float timer;

    public GameObject chainlink;
    public int chainlength;
    private List<GameObject> chains = new List<GameObject>();
    private void Start()
    {
    }

    void Update ()
    {
        pointToMouse();
        shootSpear();
        spearTimer();
    }

    void spearTimer()
    {
        if (isFired)
        {
            timer += Time.deltaTime;
            if (timer >= fire_rate)
            {
                isFired = false;
                timer = 0;
            }
        }
    }

    void pointToMouse()
    {
        float angle;
        Vector3 object_pos;
        Vector3 mouse_pos;
        
        mouse_pos = Input.mousePosition;
        mouse_pos.z = cam.transform.position.z - this.transform.position.z; //The distance between the camera and object
        object_pos = Camera.main.WorldToScreenPoint(this.transform.position);
        mouse_pos.x = mouse_pos.x - object_pos.x;
        mouse_pos.y = mouse_pos.y - object_pos.y;
        angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void shootSpear()
    {
        if (Input.GetAxis("Fire1") > 0 && !isFired)
        {
            isFired = true;
            Debug.Log("PEW");
            Transform parent = this.transform.parent.transform;
            parent.GetComponent<Rigidbody2D>().AddForce(new Vector2(this.transform.right.x, this.transform.right.y) * kickback * -1);
            createChainLinks();
        }
    }

    void createChainLinks()
    {
        for (int i = 0; i < chainlength; i++)
        {
            chains.Add(Instantiate(chainlink));
            chains[i].transform.rotation = this.transform.rotation;
            chains[i].transform.Rotate(new Vector3(0,0,180));
            chains[i].transform.position = this.transform.position;
            chains[i].GetComponent<HingeJoint2D>().enableCollision = false;
            if (i > 0)
            {
                chains[i].GetComponent<HingeJoint2D>().connectedBody =
                chains[i - 1].transform.GetComponent<Rigidbody2D>();
                chains[i].transform.position = (this.transform.position) + (this.transform.right*i*.5f);
            }
        }
        
        chains[0].GetComponent<Rigidbody2D>().AddForce(new Vector2(this.transform.right.x, this.transform.right.y) * kickback * 100);
    }

}
