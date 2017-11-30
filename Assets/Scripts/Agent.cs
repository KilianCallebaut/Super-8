using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{

    //Agent's specifics
    public AgentAttributes Attributes { get; set; }
    public AgentBehaviour Behaviour { get; set; }

    // Agent's goals
    public Vector3 Destination { get; set; }
    public Vector3 LookingDestination { get; set; }
    public Target TargetAgent { get; set; }

    // Percepts
    public Dictionary<string, OtherAgent> seenOtherAgents { get; private set; }
    public Vector3 direction { get; private set; }
    public Vector3 visionDirection { get; private set; }

    // Agent info
    public int Team { get; set; }
    private Weapon weapon;
    public Group AgentGroup { get; set; }

    private GameObject bullet;

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
        seenOtherAgents = new Dictionary<string, OtherAgent>();
        weapon = transform.Find("arm_left/gun").gameObject.AddComponent<Weapon>();
        weapon.projectileType = bullet;
		weapon.spread = 0.0f;
		weapon.shotsPerVolley = 10;
		weapon.timeBetweenShots = 0.1f;
		gameObject.AddComponent<DamageRecipient>();
        
        Behaviour = gameObject.AddComponent<AgentStandardBehaviour>();
        Destination = transform.position;
        visionDirection = gameObject.GetComponent<Rigidbody2D>().transform.forward;
    }


    // Update is called once per frame
    void Update() {
		int dam = GetComponent<DamageRecipient> ().damageReceived;
		if (health <= dam) {
			Die ();
		} else {
			See();
			Behaviour.Think();
			Act();
		}
    }

    private void See()
    {
        // update direction 
        if (!AtDestination())
        { 
            var heading = (Destination - transform.position);
            direction = heading / heading.magnitude;
        }


        // See enemies
        Spotting();

        // Aim at target
        Aiming();

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
            OtherAgent oa = new OtherAgent(a.name, a.Team, a.transform.position);

            if (InFieldOfVision(a))
            {
                AgentGroup.SeenAgent(oa);

                // Update position
                if (seenOtherAgents.ContainsKey(oa.Name))
                    seenOtherAgents[oa.Name].Position = oa.Position;
                else
                    seenOtherAgents.Add(oa.Name, oa);

                if (TargetAgent != null && TargetAgent.Enemy.Equals(oa))
                {
                    TargetAgent.Enemy.Position = oa.Position;
                    TargetAgent.Seen = true;
                }
            }
            else 
            {
                AgentGroup.UnseenAgent(oa);

                // Remove unseen agents
                seenOtherAgents.Remove(oa.Name);
                if (TargetAgent != null && TargetAgent.Enemy.Equals(oa))
                {
                    TargetAgent.Seen = false;
                }
            }
        }

        // Remove deleted agents
        /*
        foreach (KeyValuePair<string, OtherAgent> oa in seenOtherAgents)
        {
            var tosearch = LevelManager.Instance.Agents.Find(x => x.name == oa.Key);
            if (tosearch == null)
            {
                seenOtherAgents.Remove(oa.Key);
                if (oa.Key == TargetAgent.Enemy.Name)
                {
                    TargetAgent = null;
                }
            }
        }*/

        
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
        return Physics2D.Linecast(transform.position, a.transform.position, LayerMask.GetMask("Walls"));
        
        
    }

    // Checks if the agent is at its destination
    public bool AtDestination()
    {
        return transform.position == Destination;
    }

    // Placeholder for collisions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        Stop();
    }
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Stop();
        Debug.Log(collision.transform.tag);
    }
    


    //Actions
    // Placeholder for movement
    private void MoveToDirection()
    {
      


        if (transform.position != Destination) { 
            transform.position = Vector2.MoveTowards(transform.position, Destination, Attributes.speed * Time.deltaTime);
			//experiment: use rigidbody2d for velocity instead
			//GetComponent<Rigidbody2D>().velocity = (Destination-transform.position).normalized*Attributes.speed*Time.deltaTime;
        }
    }
    
    // Placeholder for shooting
    private void Shoot()
    {
        if (TargetAgent != null)
        {
            // Update direction
            float offSet = Mathf.Pow(Vector3.Distance(transform.position, TargetAgent.Enemy.Position), 2.0f) / Attributes.accuracy;
            offSet -= aimBonus;
			Vector3 shootingLocation = TargetAgent.Enemy.Position; //(Vector3) Random.insideUnitCircle * offSet + TargetAgent.Enemy.Position;

			Debug.Log (shootingLocation);

			weapon.setShootingDirection (shootingLocation);
			//weapon.setShootingDirection	(new Vector3(shootingLocation.x, 0.0f, shootingLocation.y));
            TargetAgent.AimTime = Time.time;

			weapon.startShooting ();

        }

        
        if (TargetAgent == null && weapon.isShooting())
        {
            weapon.stopShooting();
        }

       
    }


    // Placeholder for dying
    private void Die()
    {
        AgentGroup.DeleteMember(this);
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

    // For debugging
    void OnDrawGizmosSelected()
    {
        Debug.Log(AtDestination());
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(Destination, 0.4f);

        // For debugging purposes, shows field of vision
        var forwardpoint = (visionDirection * Attributes.reachOfVision);
        Debug.DrawLine(transform.position, (visionDirection * Attributes.reachOfVision) + transform.position, Color.red);
        var rotatedforwardpoint = Quaternion.Euler(0, 0, Attributes.widthOfVision) * forwardpoint + transform.position;
        Debug.DrawLine(transform.position, rotatedforwardpoint);
        var rotatedforwardpoint2 = Quaternion.Euler(0, 0, -Attributes.widthOfVision) * forwardpoint + transform.position;
        Debug.DrawLine(transform.position, rotatedforwardpoint2);
        Debug.DrawLine(transform.position, Destination, Color.blue);

        var aimZone = (Mathf.Pow(Vector3.Distance(transform.position, TargetAgent.Enemy.Position), 2.0f) / Attributes.accuracy) - aimBonus;
        var aimlimitleft = TargetAgent.LastPosition + new Vector3(-aimZone, 0);
        var aimlimitright = TargetAgent.LastPosition + new Vector3(aimZone, 0);
        var aimlimitup = TargetAgent.LastPosition + new Vector3(0, aimZone);
        var aimlimitdown = TargetAgent.LastPosition + new Vector3(0, -aimZone);

        Debug.DrawLine(aimlimitleft, aimlimitup);
        Debug.DrawLine(aimlimitleft, aimlimitdown);
        Debug.DrawLine(aimlimitright, aimlimitup);
        Debug.DrawLine(aimlimitright, aimlimitdown);

        Debug.Log(TargetAgent);
        // Display the explosion radius when selected
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(TargetAgent.LastPosition, 0.1f);

        


    }
}
