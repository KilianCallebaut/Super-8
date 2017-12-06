using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/**
 * Bullet sponge that also deals out a lot of damage to agents in front of him.
 * Defender
 * */
public class HeavyRole : AgentBehaviour
{

    private float inAreaThreshold = 4.0f;
    int flankingSide = 0;

    // Use this for initialization
    void Start () {
		
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
        Engaging();


    }

    // When spotting an enemy agent, stop (when in targetarea)
    // Since the only thing that is really meant to change here is the destination, we can order
    // Goal: position so small flanking options + best area covering
    protected override void Positioning()
    {
        switch (positioning)
        {
            case PositioningMethod.GoToGroupObjective:
                GoToGroupObjective();
                break;
            case PositioningMethod.HittingTheBack:
                HittingTheBack();
                break;
            case PositioningMethod.Flanking:
                flankingSide = Flanking(flankingSide);
                break;
            case PositioningMethod.Retreat:
                Retreat();
                break;
            case PositioningMethod.StayInGroup:
                StayInGroup();
                break;
            case PositioningMethod.Chasing:
                Chasing();
                break;
            default:
                Stop();
                break;

        }


        if (agent.AtDestination())
        {
            GoToGroupObjective();
            StayInGroup();

        }

        if (InObjectiveArea())
        {
            OtherAgent enemy = agent.seenOtherAgents.Where(x => x.Value.Team != agent.Team).FirstOrDefault().Value;
            if (enemy != null)
            {
                Stop();
            }
        }


    }


    // Targets agent closest to vision direction
    // Only meant to change the agents Target
    protected override void Targetting()
    { 

        if (agent.seenOtherAgents.Count > 0)
        {

            Prioritizing();
        }

    }


    // Looks forward when no target is selected
    // GOAL: looks at openings to anticipate attackers
    // Only meant to change the LookingDestination
    protected override void Inspecting()
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
        foreach (KeyValuePair<string, OtherAgent> a in agent.AgentGroup.SharedSeenOtherAgents)
        {
            if (a.Value.Team != agent.Team && !agentIsTarget(a.Value))
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

    // Decide when to shoot
    // For now always when agent has a target
    private void Engaging()
    {
        if (agent.TargetAgent != null)
        {
            agent.Shoot();
        } else
        {
            agent.DontShoot();
        }

    }

  
}
