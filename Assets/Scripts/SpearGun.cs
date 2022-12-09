using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class SpearGun : MonoBehaviour
{
    public GameObject cam;
    public float kickback = 2000;
    private bool isFired = false;
    public float fire_rate = 1;
    private float timer;

    public GameObject chainlink;
    public GameObject spearHead;
    public int chainlength;
    public float chainMass;
    private List<GameObject> chains = new List<GameObject>();

    private bool isUraveling = false;
    private float spawn_distance = .6f;

    void Update ()
    {
        pointToMouse();
        shootSpear();
        spearTimer();
        unravelSpear();
    }

    void unravelSpear()
    {
        if (!isUraveling)
        {
            return;
        }

        if (chains.Count > chainlength)
        {
            isUraveling = false;
            DistanceJoint2D distance_joint = chains[chains.Count-1].AddComponent<DistanceJoint2D>();
            distance_joint.connectedBody = this.transform.parent.GetComponent<Rigidbody2D>();
            distance_joint.distance = 0;
            return;
        }

        Vector3 starting_vector = this.transform.position;
        Vector3 ending_vector = chains[chains.Count - 1].transform.position;

        float distance = Vector3.Distance(this.transform.position, chains[chains.Count-1].transform.position);
        //Debug.Log(distance);
        float number_of_chain_spawns =  distance/spawn_distance;
        number_of_chain_spawns = (float)Math.Floor((double)number_of_chain_spawns);
        
        
        float y_slope = (starting_vector.y - ending_vector.y)/distance;
        float x_slope = (starting_vector.x - ending_vector.x)/distance;

        if (number_of_chain_spawns > 0)
        {
            //Vector3.Normalize(this.transform.position - chains[chains.Count - 1].transform.position);
            for (int i = 0; i < number_of_chain_spawns; i++)
            {
                //Time.timeScale = .1f;
                
                Debug.Log("distance: " + distance + " /spawn distance: " + spawn_distance + "= number of chains:" +
                          number_of_chain_spawns);

                chains.Insert(chains.Count,InstantiateNewChain(chains[chains.Count-1]));



                Vector3 chain_position = new Vector3(x_slope*(distance/number_of_chain_spawns)*i, y_slope*(distance/number_of_chain_spawns)*i, 0) + starting_vector;
                chains[chains.Count - 1].transform.position = chain_position;
                chains[chains.Count - 1].transform.rotation = this.transform.rotation;
                chains[chains.Count - 1].GetComponent<Rigidbody2D>().AddForce((kickback/chains.Count) * this.transform.right);
            }
        }

        float avg_mass = chainMass / chains.Count;
        foreach (GameObject g in chains)
        {
            g.GetComponent<Rigidbody2D>().mass = avg_mass;
        }

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
            parent.GetComponent<Rigidbody2D>().AddForce(new Vector2(this.transform.right.x, this.transform.right.y) * kickback * -1 * (1));
            //createChainLinksInstant();
            startUnravelChains();
        }
        else if(Input.GetAxis("Fire2") > 0)
        {
            isUraveling = false;
            clearChainsInstant();
        }
    }

    private void startUnravelChains()
    {
        isUraveling = true;
        if (chains.Count == 0)
        {
            chains.Add(InstantiateNewChain());
            chains[0].GetComponent<Rigidbody2D>()
                .AddForce(new Vector2(this.transform.right.x, this.transform.right.y) * kickback);
            chains[0].GetComponent<HingeJoint2D>().enabled = false;
        }
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
    
    private GameObject InstantiateNewChain(GameObject last_chain)
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
        
        new_chain.transform.position = (transform.position);
        return new_chain;

    }
    
    private GameObject InstantiateNewChain()
    {
        //create new chain
        GameObject new_chain = Instantiate(spearHead);
            
        //rotate in direction
        new_chain.transform.rotation = this.transform.rotation;
        new_chain.transform.Rotate(new Vector3(0,0, 0));
            
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
