using System;
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
    public Vector3 direction { get;  set; }
    public Vector3 visionDirection { get; private set; }
    public Vector3 lastPos { get; set; }

    // Agent info
    public int Team { get; set; }
    public Weapon weapon { get; set; }
    public Group AgentGroup { get; set; }

    private GameObject bullet;
    private Unit unit;

    // Agent's bonusses
    private float aimBonus = 0.0f;
    private float aimZoneMin = 0.7f;
    private float aimZoneMax = 0.7f + 8.0f;
    private float aimZone = 1.87f / 2.0f;
    private float aimIncrease = 0.1f;

    private float health = 0.0f;
    private bool shadow = false;
    public bool Shadow
    {
        get
        {
            return shadow;
        }

        set
        {
            shadow = value;
            if (value == true)
            {
                LevelManager.Instance.ShadowMap.Add(this, new Dictionary<Agent, int>());
            }
            else
                LevelManager.Instance.ShadowMap.Remove(this);
        }
    }

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


		gameObject.AddComponent<DamageRecipient>();
        unit = gameObject.GetComponent<Unit>();


        Destination = transform.position;
        //visionDirection = gameObject.GetComponent<Rigidbody2D>().transform.forward;
        direction = new Vector3(-1f, 0f, 0f);
        visionDirection = direction;
        lastPos = transform.position;

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
            lastPos = transform.position;

        }
    }

    private void See()
    {


        // See enemies
        Spotting();

        // Aim at target
        Aiming();

    }

    private void Act()
    {
        MoveToDirection();
        RotateToDirection();
        
    }

    // Percepts
    private void Spotting()
    {
        foreach (Agent a in LevelManager.Instance.Agents)
        {
            OtherAgent oa = new OtherAgent(a.name, a.Team, a.transform.position, a.visionDirection, a.direction);
            if (InFieldOfVision(a.name))
            {


                // Update position & looking direction
                if (seenOtherAgents.ContainsKey(oa.Name))
                {
                    seenOtherAgents[oa.Name].Position = oa.Position;
                    seenOtherAgents[oa.Name].VisionDirection = oa.VisionDirection;
                }
                else
                {
                    seenOtherAgents.Add(oa.Name, oa);
                    AgentGroup.SeenAgent(oa);
                }

                if (TargetAgent != null && TargetAgent.Enemy.Equals(oa))
                {
                    TargetAgent.Enemy.Position = oa.Position;
                }
            }
            else
            {
                AgentGroup.UnseenAgent(oa);

                // Remove unseen agents
                seenOtherAgents.Remove(oa.Name);

            }
        }
        
    }

    /** 
     * Calculates bonus for aiming for an enemy
     * increases the more an enemy is in the middle of his vision
     **/
    private void Aiming()
    {
        if (TargetAgent != null && InFieldOfVision(TargetAgent.Enemy.Name))
        {
            aimZone = aimZoneMax;
            // Bonus for accuracy
            // 
            var aimBonusAcc = Attributes.accuracy / 10.0f;
            aimZone -= (aimZoneMax - aimZoneMin) * (aimBonusAcc/2.0f);

            // Bonus for distance
            var aimBonusDist = 1.0f - (Vector3.Distance(transform.position, TargetAgent.LastPosition) / Attributes.reachOfVision);
            aimZone = aimZone - (aimZone - aimZoneMin) * aimBonusDist;

            // Penalty for being away from center
            // 30%
            Vector3 enemyDirection = (TargetAgent.Enemy.Position - transform.position).normalized;
            var aimPenaltyAng = Vector2.Angle(enemyDirection, visionDirection) / Attributes.widthOfVision;
            aimZone += (aimZone - aimZoneMin) * aimPenaltyAng;



            // Bonus for standing still
            // 20% 
            var aimBonusStan = 0.0f;
            if (Destination == transform.position)
                aimBonusStan = 0.3f;

            aimZone -= (aimZone - aimZoneMin) * aimBonusStan;

            if (aimZone < aimZoneMin)
                aimZone = aimZoneMin;





        }
    }


    // Defines if agent is seen or not, search by name
    public bool InFieldOfVision(string name)
    {
        return LevelManager.Instance.CanSee(this, name);
    }

    // Defines if agent is seen or not, search by name
    public bool InFieldOfVision(Vector3 position)
    {
        return LevelManager.Instance.CanSee(this, position);
    }



    // Checks if the agent is at its destination
    public bool AtDestination()
    {

        return Vector2.Distance(transform.position,Destination) < 0.8f ;
    }

    // Checks if the agent is looking at its targetdirection
    public bool LookingInRightDirection()
    {
        var TargetDirection = (LookingDestination - transform.position).normalized;

        float ang = Vector3.Angle(TargetDirection, visionDirection);
        if (ang < 1.0f)
        {
            return true;
        }
        return false;
    }

    // Placeholder for collisions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        Stop();
    }
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Agent isagent = collision.gameObject.GetComponent<Agent>();
        if (isagent != null && isagent.Team == Team)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);

        }
        
    }
    


    //Actions

    // Updates
    
    // Placeholder for movement
    private void MoveToDirection()
    {     
        unit.Destination = Destination;

    }

    // Placeholder for rotating vision
    private void RotateToDirection()
    {

        if (LookingDestination != transform.position)
        {
            var TargetDirection = (LookingDestination - transform.position).normalized;

            
            if (!LookingInRightDirection())
            {
                visionDirection = Vector3.RotateTowards(visionDirection, TargetDirection, Attributes.agility * Time.deltaTime, 0.0f);
            }
        }


    }

    // Agent Actions

    // Letting agent shoot
    public void Shoot()
    {

        if (TargetAgent != null)
        {
            
            // Update direction
            Vector3 enemyDirection = (TargetAgent.Enemy.Position - transform.position);
          
			Vector3 shootingLocation = (Vector3) UnityEngine.Random.insideUnitCircle * aimZone + TargetAgent.Enemy.Position + TargetAgent.Enemy.Direction;

			weapon.setShootingDirection (shootingLocation);

			weapon.startShooting ();

            if (Shadow)
            {
                Shadow = false;
            }
        } else
        {
            DontShoot();
        }

        

    }

    // Stops agent from shooting
    public void DontShoot()
    {
        if (weapon.isShooting())
        {
            weapon.stopShooting();
        }

    }

    // Placeholder for dying
    private void Die()
    {
        foreach(OtherAgent oa in seenOtherAgents.Values)
        {
            AgentGroup.UnseenAgent(oa);
        }
        AgentGroup.DeleteMember(this);
        LevelManager.Instance.DeleteAgent(this);
    }

   

    // Placeholder for stopping
    private void Stop()
    {
        Destination = transform.position;
    }

    // For debugging
    void OnDrawGizmosSelected()
    {
        //Debug.Log(AtDestination());
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(Destination, 0.1f);

        // For debugging purposes, shows field of vision
        Debug.DrawLine(new Vector3(5.0f, 160f, 0f), new Vector3(105.0f, 160f, 0f)) ;
        var forwardpoint = (visionDirection * Attributes.reachOfVision);
        Debug.DrawLine(transform.position, (visionDirection * Attributes.reachOfVision) + transform.position, Color.red);
        var rotatedforwardpoint = Quaternion.Euler(0, 0, Attributes.widthOfVision) * forwardpoint + transform.position;
        Debug.DrawLine(transform.position, rotatedforwardpoint);
        var rotatedforwardpoint2 = Quaternion.Euler(0, 0, -Attributes.widthOfVision) * forwardpoint + transform.position;
        Debug.DrawLine(transform.position, rotatedforwardpoint2);
        //Debug.DrawLine(transform.position, Destination, Color.blue);

        

        if (TargetAgent != null)
        {
            Vector3 enemyDirection = (TargetAgent.Enemy.Position - transform.position);
            Vector3 shootingDirectionLimit = Quaternion.Euler(0, 0, Attributes.accuracy) * enemyDirection;
            var aimlimitleft = TargetAgent.LastPosition + new Vector3(-aimZone, 0);
            var aimlimitright = TargetAgent.LastPosition + new Vector3(aimZone, 0);
            var aimlimitup = TargetAgent.LastPosition + new Vector3(0, aimZone);
            var aimlimitdown = TargetAgent.LastPosition + new Vector3(0, -aimZone);
            //Debug.Log(aimZone);


            Debug.DrawLine(aimlimitleft, aimlimitup);
            Debug.DrawLine(aimlimitleft, aimlimitdown);
            Debug.DrawLine(aimlimitright, aimlimitup);
            Debug.DrawLine(aimlimitright, aimlimitdown);

            Debug.Log(TargetAgent.Enemy.Name);
            // Display the explosion radius when selected
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(TargetAgent.LastPosition, 0.1f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(LookingDestination, 0.3f);


    }
}
