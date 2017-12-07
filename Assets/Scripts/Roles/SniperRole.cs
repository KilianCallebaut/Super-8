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

        if (agent.TargetAgent != null 
            && agent.InFieldOfVision(agent.TargetAgent.LastPosition) 
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
            case PositioningMethod.MoveToLocationWhereTargetIsVisible:
                MoveToLocationWhereTargetIsVisible();
                break;
            case PositioningMethod.GetOverview:
                GetOverview();
                break;
            default:
                Stop();
                break;

        }


        if (positioning == PositioningMethod.Stop)
        {

            if (Chasing())
                return;

            if (Retreat())
                return;
            

            if (GoToGroupObjective())
                return;

            if (GetOverview())
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

        if (Mathf.Abs(agent.AgentGroup.Objectives[0].y) <5.0f)
        {
            Vector3 dest = LevelManager.Instance.LongRangeSpotsMid[0].transform.position;
            foreach (GameObject g in LevelManager.Instance.LongRangeSpotsMid)
            {
                if (Vector2.Distance(g.transform.position, transform.position) < Vector2.Distance(dest, transform.position))
                    dest = g.transform.position;
            }
            agent.Destination = dest;
        }

        
        if (agent.TargetAgent != null)
        {
            Stop();
        }

        if (EnemiesAlmostNear())
            Stop();

        if (agent.InFieldOfVision(agent.AgentGroup.Objectives[0]) && Vector2.Distance(transform.position, agent.AgentGroup.Objectives[0]) < longRange)
        {
            Stop();
        }

        return positioning == PositioningMethod.GoToGroupObjective;

    }


    private bool Retreat()
    {
        ((AgentBehaviour)this).Retreat();

        if (agent.TargetAgent == null)
            Stop();

        if (!EnemiesNear())
            Stop();

        return positioning == PositioningMethod.Retreat;
    }

 

    
    private bool GetOverview()
    {

        if (agent.AtDestination() && Vector2.Distance(transform.position, agent.AgentGroup.Objectives[0]) < longRange)
        {
            ((AgentBehaviour)this).GetOverview();
        }

        if (agent.AtDestination())
        {
            agent.direction = (agent.AgentGroup.Objectives[0] - transform.position).normalized;

        }
        if (agent.TargetAgent != null)
            Stop();

        return positioning == PositioningMethod.GetOverview;
    }

    private bool Chasing()
    {
        ((AgentBehaviour)this).Chasing();

        if (agent.TargetAgent == null)
        {
            Stop();
            return false;
        }


        if (agent.seenOtherAgents.ContainsKey(agent.TargetAgent.Enemy.Name))
        {
            Stop();
            return false;
        }



        return positioning == PositioningMethod.Chasing;

    }
}
