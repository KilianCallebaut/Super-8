using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/**
 * Bullet sponge that also deals out a lot of damage to agents in front of him.
 * */
public class HeavyRole : AgentBehaviour
{

    private Agent agent;
    private float inAreaThreshold = 4.0f;

    // Use this for initialization
    void Start () {
		gameObject.GetComponent<Weapon> ().beHeavyMinigun ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Think()
    {
        // Positions and shoots at the enemy closest to his visiondirection
        if (agent == null)
            agent = GetComponent<Agent>();

        Inspecting();
        Targetting();
        Positioning();

    }

    // When spotting an enemy agent, stop (when in targetarea)
    // Since the only thing that is really meant to change here is the destination, we can order
    // Goal: position so small flanking options + best area covering
    private void Positioning()
    {
        StayInGroup();

        if (InObjectiveArea())
        {
            OtherAgent enemy = agent.seenOtherAgents.Where(x => x.Value.Team != agent.Team).FirstOrDefault().Value;
            if (enemy != null)
            {
                agent.Destination = agent.transform.position;
            }
            
        } 
    }


    // Targets agent closest to vision direction
    // Only meant to change the agents Target
    private void Targetting()
    { 
        if (agent.seenOtherAgents.Count > 0)
        {

            Prioritizing();
        }

    }


    // Looks forward when no target is selected
    // GOAL: looks at openings to anticipate attackers
    // Only meant to change the LookingDestination
    private void Inspecting()
    {
        if (agent.TargetAgent == null && !agent.AtDestination())
        {
            agent.LookingDestination = agent.Destination;
        }

        if (agent.TargetAgent != null)
        {
            agent.LookingDestination = agent.TargetAgent.LastPosition;
        }
    }

    // Checks if the agent is in the objective area
    private bool InObjectiveArea()
    {
        return Vector3.Distance(transform.position, agent.AgentGroup.Objectives[0]) < inAreaThreshold;
    }

    // Prioritizes targets
    private void Prioritizing()
    {
        foreach (KeyValuePair<string, OtherAgent> a in agent.seenOtherAgents)
        {
            if (a.Value.Team != agent.Team)
            {
                if (agent.TargetAgent == null)
                {
                    agent.TargetAgent = new Target(a.Value, Time.time);
                    continue;
                }

                // Calculate angle
                Vector3 enemyDirection = (a.Value.Position - transform.position).normalized;
                Vector3 targetDirection = (agent.TargetAgent.LastPosition - transform.position).normalized;


                if (Vector3.Angle(agent.visionDirection, enemyDirection) < Vector3.Angle(agent.visionDirection, targetDirection))
                {
                    agent.TargetAgent = new Target(a.Value, Time.time);
                }
            }
        }
    }

    // When he strays to far away from the group go to the leader
    private void StayInGroup()
    {
        if (Vector2.Distance(transform.position, agent.AgentGroup.Leader.transform.position) > agent.AgentGroup.Closeness)
        {
            agent.Destination = agent.AgentGroup.Leader.transform.position;
        }
        else
        {
            GoToGroupObjective();
        }
    }

    // Go to grouplocation
    private void GoToGroupObjective()
    {
        if (agent.AgentGroup.Objectives.Count > 0)
        {
            agent.Destination = agent.AgentGroup.Objectives[0];
        }
        
    }
}
