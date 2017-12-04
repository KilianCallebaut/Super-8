using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowRole : AgentBehaviour {

    int flankingSide = 0;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Move to enemy's back, stay in back of group, keep overview
    public override void Think()
    {
        // Positions and shoots at the enemy closest to his visiondirection
        if (agent == null)
            agent = GetComponent<Agent>();

        
        Inspecting();
        Positioning();
        Targetting();
        Engaging();

    }

    // Checks flanks when nog target
    protected override void Inspecting()
    {
        if (agent.TargetAgent == null)
        {
            CheckFlanks();
        }

        if (agent.TargetAgent != null)
        {
            agent.LookingDestination = agent.TargetAgent.LastPosition;
        }

       

    }

    // Stays in group, goes to enemy flank
    protected override void Positioning()
    {
        Debug.Log(positioning);
        Debug.Log(agent.Shadow);
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
        Debug.Log(agent.Shadow);
        if (positioning == PositioningMethod.Stop)
        {
            if (HittingTheBack())
                return;
            if (GoToGroupObjective())
                return;

        }

    }

    // Prefers targets further away
    protected override void Targetting()
    {

        if (agent.seenOtherAgents.Count > 0)
        {
            if (agent.TargetAgent == null) 
                Prioritizing();

            if (agent.TargetAgent != null && InEnemyBack(agent.TargetAgent.Enemy))
            {
                agent.Shadow = false;
            }

        }
        if (!agent.Shadow && !InAnyEnemyFieldOfVision())
        {
            agent.Shadow = true;
        }

    }

    // Go after closest agent that is closer than a threshold
    private void Prioritizing()
    {
        foreach (KeyValuePair<string, OtherAgent> a in agent.AgentGroup.SharedSeenOtherAgents)
        {
            if (a.Value.Team != agent.Team && !agentIsTarget(a.Value) )
            {
                if (agent.TargetAgent == null)
                {
                    agent.TargetAgent = new Target(a.Value, Time.time);
                    continue;
                }

                if (Vector3.Distance(transform.position, a.Value.Position) > Vector3.Distance(transform.position, agent.TargetAgent.LastPosition))
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
        if (positioning != PositioningMethod.HittingTheBack)
        {
            agent.Shoot();
        }
        else
        {
            agent.DontShoot();
        }
    }

    // --------------------------------------------------------------
    // --------------------------------------------------------------
    // Positioning methods


    private bool GoToGroupObjective()
    {
        ((AgentBehaviour) this).GoToGroupObjective();
        
        StayInGroup();
        if (agent.TargetAgent != null)
        {
            Stop();
            
        }
        return positioning == PositioningMethod.GoToGroupObjective;

    }

    private bool StayInGroup()
    {
        ((AgentBehaviour)this).StayInGroup();

        if (agent.TargetAgent != null)
        {
            Stop();

        }
        return positioning == PositioningMethod.StayInGroup;

    }

    private bool HittingTheBack()
    {
        ((AgentBehaviour)this).HittingTheBack();

        if (agent.TargetAgent == null)
        {
            Stop();
        }
        Debug.Log("Hittingback " + agent.Shadow + (positioning == PositioningMethod.HittingTheBack));
        return positioning == PositioningMethod.HittingTheBack;

    }

}