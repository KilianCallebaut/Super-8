﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AgentBehaviour : MonoBehaviour {

    protected Agent agent;
    protected static float closeRange = 2.0f;
    protected static float midRange = 5.0f;
    protected static float longRange = 15.0f;
    private float acceptingThreshold = 0.01f;
    private float extraOutOfVisionDegrees = 5.0f;




    protected Vector3 turningDestination;


    // Use this for initialization
    void Start () {
        agent = gameObject.GetComponent<Agent>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    abstract public void Think();
    abstract protected void Inspecting();
    abstract protected void Targetting();
    abstract protected void Positioning();

    // Measures the level of threat an enemy poses to agent
    // TODO: balance out threat measurement
    protected float MeasureThreatLevel(OtherAgent oa)
    {
        float distanceThreat = Vector3.Distance(oa.Position, agent.transform.position);
        distanceThreat = distanceThreat / agent.Attributes.reachOfVision;

        float angleThreat = Vector3.Angle(agent.direction, (oa.Position - transform.position).normalized);
        angleThreat = angleThreat / 180.0f;

        return 0.0f;

    }
    
    // Checks if the agent is the target
    protected bool agentIsTarget(OtherAgent oa)
    {
        if (agent.TargetAgent != null && oa.Equals(agent.TargetAgent.Enemy))
        {
            return true;
        }
        return false;
    }

    // Checks if player is in enemy's breadth of vision
    protected bool InEnemyFieldOfVision(OtherAgent Enemy)
    {
        var directionToPlayer = (transform.position - Enemy.Position).normalized;

        if (Vector3.Angle(directionToPlayer, Enemy.VisionDirection) < agent.Attributes.widthOfVision + extraOutOfVisionDegrees)
        {
            return true;
        }
        return false;
    }

    // Checks if player is in enemy's breadth of vision
    protected bool InEnemyBack(OtherAgent Enemy)
    {
        var directionToPlayer = (transform.position - Enemy.Position).normalized;
        if (Vector3.Angle(directionToPlayer, -Enemy.Direction) < agent.Attributes.widthOfVision + extraOutOfVisionDegrees)
        {
            return true;
        }
        return false;
    }

    // Checks if agent is acceptable enough at point
    protected bool IsAtPoint(Vector3 point)
    {
        if(Vector3.Distance(transform.position, point) < acceptingThreshold)
        {
            return true;
        }
        return false;
    }

    // check if any enemy is closer than closerange
    protected bool EnemiesVeryNear()
    {
        return agent.seenOtherAgents.Any(x => ((Vector3.Distance(x.Value.Position, transform.position) < closeRange) && (x.Value.Team != agent.Team)));

    }

    // Check if any enemy is closer than midrange
    protected bool EnemiesNear()
    { 
        return agent.seenOtherAgents.Any(x => ((Vector3.Distance(x.Value.Position, transform.position) < midRange) && (x.Value.Team != agent.Team)));

    }

    // Check if any enemy is closer than longrange
    protected bool EnemiesAlmostNear()
    {
        return agent.seenOtherAgents.Any(x => ((Vector3.Distance(x.Value.Position, transform.position) < longRange) && (x.Value.Team != agent.Team)));

    }

    // Inspecting methods

    // Look around in an angle of the current direction
    protected void LookAround()
    {
        agent.LookingDestination = Quaternion.Euler(0, 0, agent.Attributes.widthOfVision) * agent.visionDirection * agent.Attributes.reachOfVision + transform.position;

    }

    // Look to the flanks
    protected void CheckFlanks()
    {
        Vector3 forward = agent.direction;
        Vector3 left = Quaternion.Euler(0, 0, -agent.Attributes.widthOfVision) * forward;
        Vector3 right = Quaternion.Euler(0, 0, agent.Attributes.widthOfVision) * forward;
        

        if (turningDestination == Vector3.zero)
        {
            turningDestination = left;

        }
        else if (agent.LookingInRightDirection() && Vector3.Angle(left, agent.visionDirection) < Vector3.Angle(right, agent.visionDirection) )
        {

            turningDestination = right;

        }
        else if (agent.LookingInRightDirection() && Vector3.Angle(left, agent.visionDirection) > Vector3.Angle(right, agent.visionDirection))
        {

            turningDestination = Vector3.zero;
        }

        agent.LookingDestination = turningDestination * agent.Attributes.reachOfVision + transform.position;


    }


    // Postioning methods

    // Stops where he's going
    protected void Stop()
    {
        agent.Destination = transform.position;
    }


    // Going to group's objective
    protected void GoToGroupObjective()
    {
        if (agent.AgentGroup.Objectives.Count > 0)
        {
            agent.Destination = agent.AgentGroup.Objectives[0];
        }

    }

    // When he strays to far away from the group go to the leader
    protected void StayInGroup()
    {
        if (agent.AgentGroup.Leader != null &&
            Vector2.Distance(transform.position, agent.AgentGroup.Leader.transform.position) > agent.AgentGroup.Closeness)
        {
            agent.Destination = agent.AgentGroup.Leader.transform.position;
        }
    }

    // Dealing with enemies

    // Chasing placeholder
    protected void Chasing()
    {
        if (agent.TargetAgent != null)
        {
            agent.Destination = agent.TargetAgent.LastPosition;
        }
    }


    // Approach, while avoid being hit
    protected int Flanking(int flankingSide)
    {
        if (agent.TargetAgent != null )
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

            if (InEnemyFieldOfVision(agent.TargetAgent.Enemy))
            {
                if (flankingSide != 0)
                {
                    if (flankingSide == 1)
                    {
                        agent.Destination = (agent.TargetAgent.LastPosition - ClosestPoint).normalized + ClosestPoint;

                        return 1;
                    }
                    else if (flankingSide == 2)
                    {
                        agent.Destination = (agent.TargetAgent.LastPosition - ClosestPoint2).normalized + ClosestPoint2;
                        return 2;

                    }
                }

                if (Vector3.Distance(ClosestPoint, transform.position) > Vector3.Distance(ClosestPoint2, transform.position))
                {
                    agent.Destination = (agent.TargetAgent.LastPosition - ClosestPoint2).normalized + ClosestPoint2;
                    return 2;
                }
                else
                {
                    agent.Destination = (agent.TargetAgent.LastPosition - ClosestPoint).normalized + ClosestPoint;
                    return 1;
                }
            } 

        }
        return 0;
    }


    // Go to location in the agent's back
    protected void HittingTheBack()
    {
        if (agent.TargetAgent != null && agent.Shadow == true)
        {
            Vector3 oppositeDirection = -agent.TargetAgent.Enemy.Direction;

            // ToDo: avoid setting locations where there are walls
            agent.Destination = oppositeDirection * midRange + agent.TargetAgent.LastPosition;
        } 
    }

    // Retreats to position further from target
    // For now go in exact opposite direction
    protected void Retreat()
    {
        if (agent.TargetAgent != null)
        {
            var direction = (agent.TargetAgent.LastPosition - transform.position).normalized;
            var oppositeDirection = new Vector3(-direction.x, -direction.y);
            agent.Destination = oppositeDirection + transform.position;
        }
    }

    // Calculate the closest point on the side of an agent
    protected Vector3 CalculateClosestSidePoint()
    {
        Vector3 enemyDirection = agent.TargetAgent.Enemy.VisionDirection;
        Vector3 perpendicularDir1 = new Vector3(enemyDirection.y, -enemyDirection.x);
        Vector3 perpendicularDir2 = new Vector3(-enemyDirection.y, enemyDirection.x);
        Vector3 perpendicularPoint1 = perpendicularDir1 + agent.TargetAgent.LastPosition;
        Vector3 perpendicularPoint2 = perpendicularDir2 + agent.TargetAgent.LastPosition;


        var k1 = ((perpendicularPoint1.y - agent.TargetAgent.LastPosition.y) * (transform.position.x - agent.TargetAgent.LastPosition.x)
            - (perpendicularPoint1.x - agent.TargetAgent.LastPosition.x) * (transform.position.y - agent.TargetAgent.LastPosition.y))
            / (Mathf.Pow((perpendicularPoint1.y - agent.TargetAgent.LastPosition.y), 2.0f) + Mathf.Pow((perpendicularPoint1.x - agent.TargetAgent.LastPosition.x), 2.0f));

        var k2 = ((perpendicularPoint2.y - agent.TargetAgent.LastPosition.y) * (transform.position.x - agent.TargetAgent.LastPosition.x)
            - (perpendicularPoint2.x - agent.TargetAgent.LastPosition.x) * (transform.position.y - agent.TargetAgent.LastPosition.y))
            / (Mathf.Pow((perpendicularPoint2.y - agent.TargetAgent.LastPosition.y), 2.0f) + Mathf.Pow((perpendicularPoint2.x - agent.TargetAgent.LastPosition.x), 2.0f));

        var xClosest = transform.position.x - k1 * (perpendicularPoint1.y - agent.TargetAgent.LastPosition.y);
        var yClosest = transform.position.y + k1 * (perpendicularPoint1.x - agent.TargetAgent.LastPosition.x);
        Vector3 ClosestPoint = new Vector3(xClosest, yClosest, 0f);


        var xClosest2 = transform.position.x - k2 * (perpendicularPoint2.y - agent.TargetAgent.LastPosition.y);
        var yClosest2 = transform.position.y + k2 * (perpendicularPoint2.x - agent.TargetAgent.LastPosition.x);
        Vector3 ClosestPoint2 = new Vector3(xClosest2, yClosest2, 0f);

        if (Vector3.Distance(transform.position, ClosestPoint) < Vector3.Distance(transform.position, ClosestPoint2))
        {
            return ClosestPoint;
        } else
        {
            return ClosestPoint2;
        }

    }

}
