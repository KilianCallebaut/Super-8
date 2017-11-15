using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

    //Agent's specifics

    public AgentAttributes Attributes { get; set; }
    public AgentBehaviour Behaviour { get; set; }

    // Agent's goals
    public Vector3 Destination { get; set; }
    public Vector3 LookingDestination { get; set; }
    public Target TargetAgent { get; set; }

    // Percepts
    public List<OtherAgent> seenOtherAgents { get; private set; }
    public Vector3 direction { get; private set; }
    public Vector3 visionDirection { get; private set; }

    // Agent info
    public int Team { get; set; }
    private Gun gun;

    // Agent's bonusses
    private float aimBonus = 0.0f;
    private float aimIncrease = 0.1f;
    private float health = 0.0f;

    // Main loop
    // Use this for initialization
    void Start() {
        initialize();
        health = Attributes.maxHealth;
        See();
        Behaviour.Think();
    }

    private void initialize()
    {
        seenOtherAgents = new List<OtherAgent>();
        gun = GetComponent<Gun>();
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

        // Aim at target
        Aiming();

        // Delete killed agents
        seenOtherAgents.RemoveAll(delegate (OtherAgent o) { return o == null; });
        
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
        foreach (Agent a in LevelManager.Instance.Agents)
        {
            OtherAgent oa = new OtherAgent(a.name, a.transform.position);

            if (InFieldOfVision(a))
            {

                // Update position
                int index = seenOtherAgents.FindLastIndex(x => x == oa);
                if (index != -1)
                    seenOtherAgents[index].Position = oa.Position;
                else
                    seenOtherAgents.Add(oa);

                if (TargetAgent != null && TargetAgent.Enemy.Equals(oa))
                {
                    TargetAgent.Enemy.Position = oa.Position;
                    TargetAgent.Seen = true;
                }
            }
            else 
            {
                // Remove unseen agents
                seenOtherAgents.Remove(oa);
                if (TargetAgent != null && TargetAgent.Enemy.Equals(oa))
                {
                    TargetAgent.Seen = false;
                }
            }
        }

        
    }

    /** 
     * Calculates bonus for aiming for an enemy
     * increases linearly by aimIncrease/second the time the agent can see the target
     **/
    private void Aiming()
    {
        if (TargetAgent != null )
        {
            aimBonus = aimIncrease * TargetAgent.AimOffset;

            var aimZone = (Mathf.Pow(Vector3.Distance(transform.position, TargetAgent.Enemy.Position), 2.0f) / Attributes.accuracy) - aimBonus;
            var aimlimitleft = TargetAgent.LastPosition + new Vector3(-aimZone, 0);
            var aimlimitright = TargetAgent.LastPosition + new Vector3(aimZone, 0);
            var aimlimitup = TargetAgent.LastPosition + new Vector3(0, aimZone);
            var aimlimitdown = TargetAgent.LastPosition + new Vector3(0, -aimZone);

            Debug.DrawLine(aimlimitleft, aimlimitup);
            Debug.DrawLine(aimlimitleft, aimlimitdown);
            Debug.DrawLine(aimlimitright, aimlimitup);
            Debug.DrawLine(aimlimitright, aimlimitdown);


        }
    }

    // Defines if something is seen or not
    private bool InFieldOfVision(Agent a)
    {
        Vector3 objectPosition = a.transform.position;
        float distanceToObject = (objectPosition - transform.position).magnitude;
        Vector3 directionToObject = (objectPosition - transform.position) / distanceToObject;

        if (distanceToObject < Attributes.reachOfVision && Vector3.Angle(directionToObject, visionDirection) <Attributes.widthOfVision 
            && Vector3.Angle(directionToObject, visionDirection) > - Attributes.widthOfVision && !BehindObject(a.gameObject))
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
        if (collision.tag == "Bullet" || gameObject.tag == "Bullet")
        {
            health--;
            if (health == 0.0f)
            {
                Die();

            }
        }
        Stop();
    }
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
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
        if (TargetAgent != null)
        {
            float offSet = Mathf.Pow(Vector3.Distance(transform.position, TargetAgent.Enemy.Position), 2.0f) / Attributes.accuracy;
            offSet -= aimBonus;
            Vector3 shootingLocation = (Vector3) Random.insideUnitCircle * offSet + TargetAgent.Enemy.Position;
            gun.Shoot(shootingLocation);
            TargetAgent.AimTime = Time.time;
           
        }
    }


    // Placeholder for dying
    private void Die()
    {
        LevelManager.Instance.DeleteAgent(this);
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
