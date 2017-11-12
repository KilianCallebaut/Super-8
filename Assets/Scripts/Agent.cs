using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

    //Agent's specifics

    public AgentAttributes Attributes { get; set; }
    public AgentBehaviour Behaviour { get; set; }

    private Gun gun;

    // Agent's goals
    public Vector3 Destination { get; set; }
    public Vector3 LookingDestination { get; set; }
    public GameObject Target { get; set; }

    // Percepts
    public List<GameObject> seenOtherAgents { get; private set; }
    public Vector3 direction { get; private set; }
    public Vector3 visionDirection { get; private set; }

    // Agent info
    private int team;
    private AgentAttributes attributes;

    // Main loop
    // Use this for initialization
    void Start() {
        initialize();
        See();
        Behaviour.Think();
    }

    private void initialize()
    {
        seenOtherAgents = new List<GameObject>();
        gun = GetComponent<Gun>();
        attributes = new AgentAttributes();
        Behaviour = gameObject.AddComponent<AgentStandardBehaviour>();
    }


    // Update is called once per frame
    void Update() {
        See();
        Behaviour.Think();
        Act();
    }

    private void See()
    {
        // update direction 
        if (Destination != transform.position)
        { 
            var heading = (Destination - transform.position);
            direction = heading / heading.magnitude;
        }

       
        // update visionDirection 
        if (visionDirection == Vector3.zero)
        {
            var seeing = (LookingDestination - transform.position);
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
        Debug.DrawLine(transform.position, Destination, Color.blue);

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

        Stop();
    }

    


    //Actions
    // Placeholder for movement
    private void MoveToDirection()
    {
        if (transform.position != Destination) { 
            transform.position = Vector2.MoveTowards(transform.position, Destination, Attributes.speed * Time.deltaTime);
        }
    }
    
    // Placeholder for shooting
    private void Shoot()
    {
        if (Target != null)
        {
            float offSet = Mathf.Pow(Vector3.Distance(transform.position, Target.transform.position), 2.0f) / Attributes.accuracy;
            Vector3 shootingLocation = (Vector3) Random.insideUnitCircle * offSet + Target.transform.position;
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

        if (LookingDestination != transform.position)
        {
            var Targetting = (LookingDestination - transform.position);
            var TargetDirection = Targetting / Targetting.magnitude;

            if (visionDirection != TargetDirection)
            {
                visionDirection = Vector3.RotateTowards(visionDirection, TargetDirection, Attributes.agility * Time.deltaTime, 0.0f);

            }
        }

        
    }

    // Placeholder for stopping
    private void Stop()
    {
        Destination = transform.position;
    }

   
}
