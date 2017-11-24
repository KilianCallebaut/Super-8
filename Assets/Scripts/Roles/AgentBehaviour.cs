using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentBehaviour : MonoBehaviour {

    protected Agent agent;
    protected float closeRange = 1.0f;
    protected float midRange = 5.0f;
    protected float longRange = 15.0f;

    // Use this for initialization
    void Start () {
        agent = gameObject.GetComponent<Agent>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    abstract public void Think();

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

    // Checks if the agent is the target
    protected bool agentIsTarget(OtherAgent oa)
    {
        if(agent.TargetAgent != null && oa.Equals(agent.TargetAgent.Enemy))
        {
            return true;
        }
        return false;
    }
}
