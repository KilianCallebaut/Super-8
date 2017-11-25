using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentBehaviour : MonoBehaviour {

    protected Agent agent;
    protected float closeRange = 1.0f;
    protected float midRange = 5.0f;
    protected float longRange = 15.0f;
    private float acceptingThreshold = 0.01f;

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

        if (turningDestination == Vector3.zero)
        {
            turningDestination = Quaternion.Euler(0, 0, agent.Attributes.widthOfVision) * forward;

        }
        else if (Vector3.Angle(turningDestination, agent.visionDirection) < acceptingThreshold)
        {

            if (Vector3.Distance(turningDestination, Quaternion.Euler(0, 0, agent.Attributes.widthOfVision) * forward) < acceptingThreshold)
            {
                turningDestination = Quaternion.Euler(0, 0, -agent.Attributes.widthOfVision) * forward;
            }
            else if (Vector3.Distance(turningDestination, Quaternion.Euler(0, 0, -agent.Attributes.widthOfVision) * forward) < acceptingThreshold)
            {
                turningDestination = Quaternion.Euler(0, 0, agent.Attributes.widthOfVision) * forward;
            }
            else
            {
                turningDestination = Vector3.zero;
            }
        }
        else
        {
            agent.LookingDestination = turningDestination * agent.Attributes.reachOfVision + transform.position;
        }
    }


    // Postioning methods

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

    // Chasing placeholder
    protected void Chasing()
    {
        if (agent.TargetAgent != null)
        {
            agent.Destination = agent.TargetAgent.LastPosition;
        }
    }

    
}
