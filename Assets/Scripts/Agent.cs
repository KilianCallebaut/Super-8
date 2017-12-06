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
    public Vector3 direction { get; private set; }
    public Vector3 visionDirection { get; private set; }

    // Agent info
    public int Team { get; set; }
    private Weapon weapon;
    public Group AgentGroup { get; set; }

    private GameObject bullet;
    private Unit unit;

	private Animator animator;
	private bool isDead;

    // Agent's bonusses
    private float aimBonus = 0.0f;
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

        weapon = ObjectManager.AddStandardWeapon(this);
		gameObject.AddComponent<DamageRecipient>();
        unit = gameObject.GetComponent<Unit>();


        Destination = transform.position;
        //visionDirection = gameObject.GetComponent<Rigidbody2D>().transform.forward;
        direction = new Vector3(-1f, 0f, 0f);
        visionDirection = direction;

		isDead = false;
		animator = GetComponent<Animator> ();

    }
    
    // Update is called once per frame
    void Update() {
		if (isDead)
			return;

		int dam = GetComponent<DamageRecipient> ().damageReceived;
		if (health <= dam) {
			animator.SetBool("isDying", true);
			animator.SetLayerWeight (1, 0);
			isDead = true;
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
            // Bonus for being more in the center
            Vector3 enemyDirection = (TargetAgent.Enemy.Position - transform.position).normalized;
            aimBonus = Vector2.Angle(enemyDirection, visionDirection) / Attributes.widthOfVision;

            // Bonus for aiming longer before shooting

            // Bonus for standing still

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
        return transform.position == Destination;
    }

    // Checks if the agent is looking at its targetdirection
    public bool LookingInRightDirection()
    {
        var TargetDirection = (LookingDestination - transform.position).normalized;

        float ang = Vector3.Angle(TargetDirection, visionDirection);
        if (ang < 0.5)
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
        else
        {
            Stop();
        }
    }
    


    //Actions

    // Updates
    
    // Placeholder for movement
    private void MoveToDirection()
    {
      
        if (transform.position != Destination) {
            //transform.position = Vector2.MoveTowards(transform.position, Destination, Attributes.speed * Time.deltaTime);
            //experiment: use rigidbody2d for velocity instead
            //GetComponent<Rigidbody2D>().velocity = (Destination-transform.position).normalized*Attributes.speed*Time.deltaTime;

            unit.Destination = Destination;
        }


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
            Vector3 shootingDirectionLimit = Quaternion.Euler(0, 0, Attributes.accuracy) * enemyDirection;
            float offSet = Vector3.Distance(enemyDirection, shootingDirectionLimit);
            offSet -= aimBonus;
			Vector3 shootingLocation = (Vector3) UnityEngine.Random.insideUnitCircle * offSet + TargetAgent.Enemy.Position;

			weapon.setShootingDirection (shootingLocation);
            TargetAgent.AimTime = Time.time;

			weapon.startShooting ();

            if (Shadow)
            {
                Shadow = false;
                Debug.Log("HERE AGENT");
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
    public void Die()
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
            float aimZone = Vector3.Distance(enemyDirection, shootingDirectionLimit) - aimBonus;
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
