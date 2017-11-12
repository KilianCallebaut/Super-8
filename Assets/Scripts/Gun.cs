using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    // Gun specs
    private float bulletSpeed = 19.0f;
    private float fireRate = 1.0f/9.0f ; // Firerate of 9 bullets/sec

    [SerializeField]
    private Rigidbody2D bullet;

    private float lastShot;
    private bool coolDown;
    private int counter;

	// Use this for initialization
	void Start () {
        lastShot = Time.time;
        coolDown = false;
        counter = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Shooting method
    public void Shoot(Vector3 target)
    {

        if (coolDown && (Time.time-lastShot) > fireRate)
        {
            coolDown = false;
        }

        if (!coolDown)
        {
            
            Rigidbody2D newBullet = Instantiate(bullet);
            newBullet.transform.position = transform.position;
            newBullet.name = gameObject.name + " Bullet " + counter;
            counter++;
            Physics2D.IgnoreCollision(newBullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

            var heading = (target - transform.position);
            var direction = heading / heading.magnitude;

            newBullet.velocity = direction * bulletSpeed;

            coolDown = true;
            lastShot = Time.time;

        }
    }

}
