using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

    [SerializeField]
    private float speed;

    [SerializeField]
    private float reachOfVision;

    [SerializeField]
    private Rigidbody2D bullet;

    private Vector3 destination;

    // Percepts
    private List<GameObject> seenOtherAgents;
    private Vector3 direction;

    private int team;

    // Main loop
    // Use this for initialization
    void Start() {
        initialize();
        see();
        think();
    }

    private void initialize()
    {
        seenOtherAgents = new List<GameObject>();
    }

    // Update is called once per frame
    void Update() {
        see();
        think();
        moveToDirection();
    }

    private void see()
    {
        // update direction 
        var heading = (destination - transform.position);
        direction = heading / heading.magnitude;

        // See enemies
        foreach (GameObject a in LevelManager.Instance.Agents)
        {
            if(inFieldOfVision(a) && !seenOtherAgents.Contains(a))
            {
                seenOtherAgents.Add(a);
            } else if (!inFieldOfVision(a) && seenOtherAgents.Contains(a))
            {
                seenOtherAgents.Remove(a);
            }
        }


    }

    // Defines if something is seen or not
    private bool inFieldOfVision(GameObject a)
    {
        Vector3 objectPosition = a.transform.position;
        float distanceToObject = (objectPosition - transform.position).magnitude;
        Vector3 directionToObject = (objectPosition - transform.position)/distanceToObject;

        if (distanceToObject < reachOfVision && Vector3.Angle(directionToObject, direction) < 10.0f)
        {
            return true;
        }
        return false;
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
        newBullet.velocity =  direction * Bullet.Speed;
    }
}
