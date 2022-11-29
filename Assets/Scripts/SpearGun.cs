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

    private bool isUraveling = false;
    
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
            createChainLinksInstant();
        }
    }

    private void startUnravelChains()
    {
        isUraveling = true;
    }

    private GameObject InstantiateNewChain(GameObject last_chain, int chain_num)
    {
        //create new chain
        GameObject new_chain = Instantiate(chainlink);
            
        //rotate in direction
        new_chain.transform.rotation = this.transform.rotation;
        new_chain.transform.Rotate(new Vector3(0,0,180));
            
        //sets position of the chain
        new_chain.transform.position = this.transform.position;
        //sets the connection point ot all joints except 0
        new_chain.GetComponent<HingeJoint2D>().connectedBody = 
            last_chain.transform.GetComponent<Rigidbody2D>();
                
        //set position of chains one ahead of the last
        new_chain.transform.position = (transform.position) + (transform.right*chain_num*.6f);
        return new_chain;

    }
    
    private GameObject InstantiateNewChain()
    {
        //create new chain
        GameObject new_chain = Instantiate(chainlink);
            
        //rotate in direction
        new_chain.transform.rotation = this.transform.rotation;
        new_chain.transform.Rotate(new Vector3(0,0,180));
            
        //sets position of the chain
        new_chain.transform.position = this.transform.position;
        return new_chain;
    }

    private void clearChainsInstant()
    {
        if (chains.Count > 0)
        {
            while (chains.Count > 0)
            {
                GameObject temp_storage = chains[0];
                chains.Remove(chains[0]);
                Destroy(temp_storage);
            }
        }
    }

    private void createChainLinksInstant()
    {
        clearChainsInstant();

        chains = new List<GameObject>();
        for (int i = 0; i < chainlength; i++)
        {
            if (i > 0)
            {
                chains.Add(InstantiateNewChain(chains[i - 1], i));
            }
            else
            {
                chains.Add(InstantiateNewChain());
            }
        }
        
        //special settings for the zero
        chains[0].GetComponent<Rigidbody2D>().AddForce(new Vector2(this.transform.right.x, this.transform.right.y) * kickback);
        chains[0].GetComponent<HingeJoint2D>().connectedBody = this.transform.parent.GetComponent<Rigidbody2D>();
        //this.transform.GetComponent<HingeJoint2D>().connectedBody = chains[0].GetComponent<Rigidbody2D>();

        int collision_buffer = 3;
        for (int i = 0; i < collision_buffer; i++)
        {
            chains[i].GetComponent<BoxCollider2D>().enabled = false;
        }
    }

}
