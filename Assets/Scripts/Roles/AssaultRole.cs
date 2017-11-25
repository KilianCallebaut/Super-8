using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * Fast runner, avoiding bullets to deal a lot of damage closeby
 * Attacker
 * */
public class AssaultRole : AgentBehaviour {

    private float chaseThreshold = 15.0f;
    private float inAreaThreshold = 4.0f;
    private float extraOutOfVisionDegrees = 5.0f;
    // 1 -> left, 2 -> right, 0 -> not flanking
    private int flankingSide = 0;
  
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Think()
    {
        if (agent == null)
            agent = GetComponent<Agent>();

        Inspecting();
        Targetting();
        Positioning();
        
    }

    // Looks around when no target is selected
    // Only meant to change the LookingDestination
    protected override void Inspecting()
    {
        if (agent.TargetAgent == null)
        {
            LookAround();
        }

        if (agent.TargetAgent != null)
        {
            agent.LookingDestination = agent.TargetAgent.LastPosition;
        }
    }





    // Targets closest enemy under distancethreshold
    // Only meant to change the agents Target
    protected override void Targetting()
    {
        if (agent.seenOtherAgents.Count > 0)
        {
            Prioritizing();
            Engaging();
        }
    }

    // When spotting an agent that's close enough approach otherwise avoid
    // Since the only thing that is really meant to change here is the destination, we can order
    // Goal: Avoiding line of fire by taking evading route
    protected override void Positioning()
    {
        GoToGroupObjective();
        StayInGroup();
        
    
        if (agent.TargetAgent != null)
        {
            // When targets looks away from you, sneak up on him, else avoid his gaze
            if (!InEnemyFieldOfVision(agent.TargetAgent.Enemy))
            {
                flankingSide = 0;
                Chasing();
            }
            else
            {
                Approaching();
            }
        }
        else
        {
            flankingSide = 0;
        }


    }

    //Checks

    // Checks if player is in enemy's breadth of vision
    private bool InEnemyFieldOfVision(OtherAgent Enemy)
    {
        var directionToPlayer = (transform.position - Enemy.Position).normalized;

        if ( Vector3.Angle(directionToPlayer, Enemy.VisionDirection) < agent.Attributes.widthOfVision + extraOutOfVisionDegrees)
        {
            return true;
        }
        return false;
    }

    // Checks if the agent is in the objective area
    private bool InObjectiveArea()
    {
        return Vector3.Distance(transform.position, agent.AgentGroup.Objectives[0]) < inAreaThreshold;
    }

    // Positioning methods

    // Approach, while avoid being hit
    private void Approaching()
    {
        
        // go to a position on targets nearest flank

        // find visionwidth line
        var rotatedVisionPoint = Quaternion.Euler(0, 0, agent.Attributes.widthOfVision + extraOutOfVisionDegrees) * agent.TargetAgent.Enemy.VisionDirection + agent.TargetAgent.LastPosition;
        var k = ((rotatedVisionPoint.y - agent.TargetAgent.LastPosition.y) * (transform.position.x - agent.TargetAgent.LastPosition.x)
            - (rotatedVisionPoint.x - agent.TargetAgent.LastPosition.x) * (transform.position.y - agent.TargetAgent.LastPosition.y))
            / (Mathf.Pow((rotatedVisionPoint.y - agent.TargetAgent.LastPosition.y), 2.0f) + Mathf.Pow((rotatedVisionPoint.x - agent.TargetAgent.LastPosition.x), 2.0f));
        
        // find closest point to agent
        var xClosest = transform.position.x - k * (rotatedVisionPoint.y - agent.TargetAgent.LastPosition.y);
        var yClosest = transform.position.y + k * (rotatedVisionPoint.x - agent.TargetAgent.LastPosition.x);
        Vector3 ClosestPoint = new Vector3(xClosest, yClosest, 0f);

        // Find visionwidth line other side
        var rotatedVisionPoint2 = Quaternion.Euler(0, 0, -agent.Attributes.widthOfVision - extraOutOfVisionDegrees) * agent.TargetAgent.Enemy.VisionDirection + agent.TargetAgent.LastPosition;
        var k2 = ((rotatedVisionPoint2.y - agent.TargetAgent.LastPosition.y) * (transform.position.x - agent.TargetAgent.LastPosition.x)
            - (rotatedVisionPoint2.x - agent.TargetAgent.LastPosition.x) * (transform.position.y - agent.TargetAgent.LastPosition.y))
            / (Mathf.Pow((rotatedVisionPoint2.y - agent.TargetAgent.LastPosition.y), 2.0f) + Mathf.Pow((rotatedVisionPoint2.x - agent.TargetAgent.LastPosition.x), 2.0f));

        // find closest point to agent
        var xClosest2 = transform.position.x - k2 * (rotatedVisionPoint2.y - agent.TargetAgent.LastPosition.y);
        var yClosest2 = transform.position.y + k2 * (rotatedVisionPoint2.x - agent.TargetAgent.LastPosition.x);
        Vector3 ClosestPoint2 = new Vector3(xClosest2, yClosest2, 0f);

        if (flankingSide != 0)
        {
            if (flankingSide == 1)
            {
                agent.Destination = ClosestPoint;
            } else if (flankingSide == 2)
            {
                agent.Destination = ClosestPoint2;

            }
        } else {

            agent.Destination = ClosestPoint;
            flankingSide = 1;
            if (Vector3.Distance(ClosestPoint, transform.position) > Vector3.Distance(ClosestPoint2, transform.position))
            {
                agent.Destination = ClosestPoint2;
                flankingSide = 2;
            }
        }

    }

    // Targetting methods

    // Go after closest agent that is closer than a threshold
    private void Prioritizing()
    {
        foreach (KeyValuePair<string, OtherAgent> a in agent.seenOtherAgents)
        {
            if (a.Value.Team != agent.Team && !agentIsTarget(a.Value) && Vector3.Distance(transform.position, a.Value.Position) < chaseThreshold)
            {
                if (agent.TargetAgent == null)
                {
                    agent.TargetAgent = new Target(a.Value, Time.time);
                    continue;
                }

                if (Vector3.Distance(transform.position, a.Value.Position) < Vector3.Distance(transform.position, agent.TargetAgent.LastPosition))
                {
                    agent.TargetAgent = new Target(a.Value, Time.time);
                }
            }
        }
    }

    // Decide when to shoot
    // For now always when agent has a target
    private void Engaging()
    {
        if (agent.TargetAgent != null)
        {
            agent.Shoot();
        }
    }


    // Inspecting methods

   
}
