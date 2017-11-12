using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

    [SerializeField]
    private float speed;

    [SerializeField]
    private Rigidbody2D bullet;

    private Vector3 destination;

    private int team;

    // Main loop
    // Use this for initialization
    void Start() {
        think();
        shoot();

    }

    // Update is called once per frame
    void Update() {
        
        think();
        moveToDirection();
    }

    private void think()
    {
        if (destination == transform.position || destination == Vector3.zero)
        {
            moveToRandomDestination();
        }
    }

    // Percepts
    // Placeholder for collisions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        moveToRandomDestination();
    }

    // Decisions
    // Placeholder for pathfinding
    private void moveToRandomDestination()
    {
        int index = Random.Range(0, LevelManager.Instance.Tiles.Count);

        GameObject tile = (GameObject)LevelManager.Instance.Tiles[index];
        destination = tile.transform.position;
    }



    //Actions
    // Placeholder for movement
    private void moveToDirection()
    {
        if (transform.position != destination) {
            transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        }
    }
    
    // Placeholder for shooting
    private void shoot()
    {
        Rigidbody2D newBullet = Instantiate(bullet);
        newBullet.transform.position = transform.position;
        var heading = (destination - transform.position);
        var direction = heading / heading.magnitude;
        newBullet.velocity =  direction * Bullet.Speed;
    }
}
