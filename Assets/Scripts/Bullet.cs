using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public GameObject Source { get; set; }

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
            if (collision.tag != "Bullet" && collision.tag != Source.tag)
            {
                Destroy(gameObject);
            }
        } catch (Exception e)
        {
            
        }

       
    }
}
