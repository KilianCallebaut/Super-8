using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public GameObject Source { get; set; }
    private float damage = 1.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Placeholder for collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        try
        {

            switch (collision.tag)
            {
                case "Bullet":
                    break;

                case "Agent":
                    if ( collision.gameObject.GetComponent<Agent>().Team != Source.GetComponent<Agent>().Team)
                    {
                        collision.gameObject.GetComponent<Agent>().Damage(damage);
                        Destroy(gameObject);

                    }
                    break;

                default:
                    Destroy(gameObject);
                    break;

                    
            }
            if (collision.tag == "Bullet")
            {
                return;
            }

        } catch (Exception e)
        {
            
        }

       
    }
}
