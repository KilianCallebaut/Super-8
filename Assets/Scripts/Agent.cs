using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

    //Agent's specifics
    private float speed = 2.0f;
    private float reachOfVision = 5.0f;
    private float widthOfVision = 50.0f;
    private float reflex = 1.0f / 2.0f;
    private float agility = 0.5f;
    private Gun gun;

    // Agent's Decisions
    private Vector3 destination;
    private Vector3 lookingDestination;
    private GameObject target;

    // Percepts
    private List<GameObject> seenOtherAgents;
    private Vector3 direction;
    private Vector3 visionDirection;

    private int team;

    // Main loop
    // Use this for initialization
    void Start() {
        initialize();
        See();
        Think();
    }

    private void initialize()
    {
        seenOtherAgents = new List<GameObject>();
        gun = GetComponent<Gun>();

    }

    // Update is called once per frame
    void Update() {
        See();
        Think();
        Act();
    }

    private void See()
    {
        // update direction 
        var heading = (destination - transform.position);
        direction = heading / heading.magnitude;

        // update visionDirection 
        var seeing = (lookingDestination - transform.position);
        visionDirection = seeing / seeing.magnitude;


        // See enemies
        Spotting();

        // Delete killed agents
        seenOtherAgents.RemoveAll(delegate (GameObject o) { return o == null; });
        
        // For debugging purposes, shows field of vision
        var forwardpoint = (visionDirection * reachOfVision);
        Debug.DrawLine(transform.position, (visionDirection * reachOfVision) + transform.position, Color.red);
        var rotatedforwardpoint = Quaternion.Euler(0, 0, widthOfVision) * forwardpoint + transform.position;
        Debug.DrawLine(transform.position, rotatedforwardpoint);
        var rotatedforwardpoint2 = Quaternion.Euler(0, 0, -widthOfVision) * forwardpoint + transform.position;
        Debug.DrawLine(transform.position, rotatedforwardpoint2);
        Debug.DrawLine(transform.position, destination, Color.blue);

    }

  

    private void Think()
    {
        // Placeholder for pathfinding
        if (destination == transform.position || destination == Vector3.zero)
        {
            MoveToRandomDestination();
        }

        // Placeholder for targetting
        if (seenOtherAgents.Count > 0)
        {
            Shoot(seenOtherAgents[0]);
        }
        Target();
    }

    private void Act()
    {
        MoveToDirection();
        RotateToDirection();
    }

    // Percepts
    private void Spotting()
    {
        foreach (GameObject a in LevelManager.Instance.Agents)
        {
            if (InFieldOfVision(a) && !seenOtherAgents.Contains(a))
            {
                seenOtherAgents.Add(a);
            }
            else if (!InFieldOfVision(a) && seenOtherAgents.Contains(a))
            {
                seenOtherAgents.Remove(a);
            }
        }
    }

    // Defines if something is seen or not
    private bool InFieldOfVision(GameObject a)
    {
        Vector3 objectPosition = a.transform.position;
        float distanceToObject = (objectPosition - transform.position).magnitude;
        Vector3 directionToObject = (objectPosition - transform.position) / distanceToObject;

        if (distanceToObject < reachOfVision && Vector3.Angle(directionToObject, visionDirection) < widthOfVision && Vector3.Angle(directionToObject, visionDirection) > -widthOfVision)
        {
            return true;
        }
        return false;
    }


    // Placeholder for collisions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bullet" )
        {
            Die();
        }

        MoveToRandomDestination();
    }

    // Decisions
    // Placeholder for pathfinding
    private void MoveToRandomDestination()
    {
        int index = Random.Range(0, LevelManager.Instance.Tiles.Count);

        GameObject tile = (GameObject)LevelManager.Instance.Tiles[index];
        destination = tile.transform.position;
    }

    // Placeholder for targetting
    private void Target()
    {
        if (target == null && seenOtherAgents.Count == 0 && destination != transform.position)
        {
            lookingDestination = destination;
        }
        if (target == null && seenOtherAgents.Count > 0 )
        {
            target = seenOtherAgents[0];
            foreach (GameObject a in seenOtherAgents)
            {
                if (Vector3.Distance(transform.position, a.transform.position) < Vector3.Distance(transform.position, target.transform.position))
                {
                    target = a;
                }
            }
        }
        if (target != null && !seenOtherAgents.Contains(target))
        {
            target = null;
        }
        if ( target != null)
        {
            lookingDestination = target.transform.position;
        }

    }

    //Actions
    // Placeholder for movement
    private void MoveToDirection()
    {
        if (transform.position != destination) { 
            transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        }
    }
    
    // Placeholder for shooting
    private void Shoot(GameObject enemy)
    {
        if (enemy != null)
        {
            gun.shoot(enemy.transform.position);
        } 
    }

    // Placeholder for dying
    private void Die()
    {
        LevelManager.Instance.DeleteAgent(gameObject);
    }

    // Placeholder for rotating vision
    private void RotateToDirection()
    {

        var targetting = (lookingDestination - transform.position);
        var targetDirection = targetting / targetting.magnitude;

        if (visionDirection != targetDirection)
        {
            visionDirection = Vector3.RotateTowards(visionDirection, targetDirection, agility * Time.deltaTime, 0.0f);
        }
    }

   
}
