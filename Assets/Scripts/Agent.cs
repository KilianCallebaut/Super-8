using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

    //Agent's specifics
    private float speed = 4.0f;
    private float reachOfVision = 5.0f;
    private float widthOfVision = 50.0f;
    private Gun gun;

    // Agent's temporals
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
        gun = GetComponent<Gun>();
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
            if (inFieldOfVision(a) && !seenOtherAgents.Contains(a))
            {
                seenOtherAgents.Add(a);
            }
            else if (!inFieldOfVision(a) && seenOtherAgents.Contains(a))
            {
                seenOtherAgents.Remove(a);
            }
        }

        // Delete killed agents
        seenOtherAgents.RemoveAll(delegate (GameObject o) { return o == null; });
        
        // For debugging purposes, shows field of vision
        var forwardpoint = (direction * reachOfVision);
        Debug.DrawLine(transform.position, (direction * reachOfVision) + transform.position, Color.red);
        var rotatedforwardpoint = Quaternion.Euler(0, 0, widthOfVision) * forwardpoint + transform.position;
        Debug.DrawLine(transform.position, rotatedforwardpoint);
        var rotatedforwardpoint2 = Quaternion.Euler(0, 0, -widthOfVision) * forwardpoint + transform.position;
        Debug.DrawLine(transform.position, rotatedforwardpoint2);

    }

    // Defines if something is seen or not
    private bool inFieldOfVision(GameObject a)
    {
        Vector3 objectPosition = a.transform.position;
        float distanceToObject = (objectPosition - transform.position).magnitude;
        Vector3 directionToObject = (objectPosition - transform.position)/distanceToObject;

        if (distanceToObject < reachOfVision && Vector3.Angle(directionToObject, direction) < widthOfVision && Vector3.Angle(directionToObject, direction) > -widthOfVision)
        {
            return true;
        }
        return false;
    }

    private void think()
    {
        // Placeholder for pathfinding
        if (destination == transform.position || destination == Vector3.zero)
        {
            moveToRandomDestination();
        }

        // Placeholder for targetting
        if (seenOtherAgents.Count > 0)
        {
            shoot(seenOtherAgents[0]);
        }
    }

    // Percepts
    // Placeholder for collisions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bullet" )
        {
            die();
        }

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
    private void shoot(GameObject enemy)
    {
        if (enemy != null)
        {
            gun.shoot(enemy.transform.position);
        } 
    }

    // Placeholder for dying
    private void die()
    {
        LevelManager.Instance.DeleteAgent(gameObject);
    }
}
