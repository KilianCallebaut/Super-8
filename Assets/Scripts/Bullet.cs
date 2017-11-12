using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Placeholder for collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Bullet")
        { 
            Destroy(gameObject);
        }


       
    }
}
