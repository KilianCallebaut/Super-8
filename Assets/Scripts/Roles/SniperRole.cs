using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SniperRole : AgentBehaviour {

    int flankingSide = 0;


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
        Engaging();

    }

    // Screens in front of him
    protected override void Inspecting()
    {
        if (agent.TargetAgent == null)
        {
            CheckFlanks();

            if (agent.AtDestination())
                LookAround();
        }

        if (agent.TargetAgent != null)
        {
            agent.LookingDestination = agent.TargetAgent.LastPosition;
           
        }
    }

    // Targets 
    protected override void Targetting()
    {

        if (agent.seenOtherAgents.Count > 0)
        {

            Prioritizing();
        }

        if (agent.TargetAgent != null &&
    Vector2.Distance(agent.TargetAgent.LastPosition, transform.position) <= closeRange
    && !agent.seenOtherAgents.ContainsKey(agent.TargetAgent.Enemy.Name))
        {
            agent.TargetAgent = null;

        }

    }

    // Set up from a distance
    protected override void Positioning()
    {
        Debug.Log(positioning);

        switch (positioning)
        {
            case PositioningMethod.GoToGroupObjective:
                GoToGroupObjective();
                break;
            case PositioningMethod.HittingTheBack:
                HittingTheBack(flankingSide);
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


        if (positioning == PositioningMethod.Stop)
        {

            if (Retreat())
                return;

            OtherAgent enemy = agent.seenOtherAgents.Where(x => x.Value.Team != agent.Team).FirstOrDefault().Value;
            if (enemy != null)
            {
                Stop();
                return;
            }

           

            if (GoToGroupObjective())
                return;

            


        }

       


    }

    // targetting methods

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

    private void Engaging()
    {
        if (agent.TargetAgent != null)
        {
            agent.Shoot();
        }
        else
        {
            agent.DontShoot();
        }
    }

    // positioning methods

  

    // Checks if the agent is in the objective area
    private bool InObjectiveArea()
    {
        return Vector3.Distance(transform.position, agent.AgentGroup.Objectives[0]) < longRange;
    }

    // ============================================================================
    // ============================================================================

    private bool GoToGroupObjective()
    {
        ((AgentBehaviour)this).GoToGroupObjective();

        StayInGroup();
        if (agent.TargetAgent != null)
        {
            Stop();

        }

        if (EnemiesAlmostNear())
            Stop();

        return positioning == PositioningMethod.GoToGroupObjective;

    }

    private bool Retreat()
    {
        ((AgentBehaviour)this).Retreat();

        if (agent.TargetAgent == null)
            Stop();

        if (!EnemiesAlmostNear())
            Stop();

        return positioning == PositioningMethod.Retreat;
    }

}
