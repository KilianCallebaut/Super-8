using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

    //Agent's specifics


    public AgentAttributes Attributes { get; set; }

    private Gun gun;

    // Agent's Decisions
    private Vector3 destination;
    private Vector3 lookingDestination;
    private GameObject target;

    // Percepts
    private List<GameObject> seenOtherAgents;
    private Vector3 direction;
    private Vector3 visionDirection;

    // Agent info
    private int team;
    private AgentAttributes attributes;

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
        attributes = new AgentAttributes();
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
        if (destination != transform.position)
        { 
            var heading = (destination - transform.position);
            direction = heading / heading.magnitude;
        }

       
        // update visionDirection 
        if (visionDirection == Vector3.zero)
        {
            var seeing = (lookingDestination - transform.position);
            visionDirection = seeing / seeing.magnitude;
        }


        // See enemies
        Spotting();

        // Delete killed agents
        seenOtherAgents.RemoveAll(delegate (GameObject o) { return o == null; });
        
        // For debugging purposes, shows field of vision
        var forwardpoint = (visionDirection * Attributes.reachOfVision);
        Debug.DrawLine(transform.position, (visionDirection *Attributes.reachOfVision) + transform.position, Color.red);
        var rotatedforwardpoint = Quaternion.Euler(0, 0,Attributes.widthOfVision) * forwardpoint + transform.position;
        Debug.DrawLine(transform.position, rotatedforwardpoint);
        var rotatedforwardpoint2 = Quaternion.Euler(0, 0, - Attributes.widthOfVision) * forwardpoint + transform.position;
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

        Target();
    }

    private void Act()
    {
        MoveToDirection();
        RotateToDirection();
        Shoot();
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

        if (distanceToObject < Attributes.reachOfVision && Vector3.Angle(directionToObject, visionDirection) <Attributes.widthOfVision 
            && Vector3.Angle(directionToObject, visionDirection) > - Attributes.widthOfVision && !BehindObject(a))
        {
            return true;
        }
        return false;
    }

    // Checks if the gameObject is behind another one
    private bool BehindObject(GameObject a)
    {
        return Physics.Linecast(transform.position, a.transform.position);
        
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

    // Placeholder for chasing
    private void Chase()
    {

    }

    //Actions
    // Placeholder for movement
    private void MoveToDirection()
    {
        if (transform.position != destination) { 
            transform.position = Vector2.MoveTowards(transform.position, destination, Attributes.speed * Time.deltaTime);
        }
    }
    
    // Placeholder for shooting
    private void Shoot()
    {
        if (target != null)
        {
            float offSet = Mathf.Pow(Vector3.Distance(transform.position, target.transform.position), 2.0f) / Attributes.accuracy;
            Vector3 shootingLocation = (Vector3) Random.insideUnitCircle * offSet + target.transform.position;
            gun.Shoot(shootingLocation);

           
        }
    }


    // Placeholder for dying
    private void Die()
    {
        //LevelManager.Instance.DeleteAgent(gameObject);
    }

    // Placeholder for rotating vision
    private void RotateToDirection()
    {

        if (lookingDestination != transform.position)
        {
            var targetting = (lookingDestination - transform.position);
            var targetDirection = targetting / targetting.magnitude;

            if (visionDirection != targetDirection)
            {
                visionDirection = Vector3.RotateTowards(visionDirection, targetDirection, Attributes.agility * Time.deltaTime, 0.0f);

            }
        }

        
    }

   
}
