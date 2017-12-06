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
    private float flankThreshold = longRange;
    private float inAreaThreshold = 4.0f;
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
        Engaging();
    }

    // Looks around when no target is selected
    // Only meant to change the LookingDestination
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





    // Targets closest enemy under distancethreshold
    // Only meant to change the agents Target
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
            

        }

    }

    // When spotting an agent that's close enough approach otherwise avoid
    // Since the only thing that is really meant to change here is the destination, we can order
    // Goal: Avoiding line of fire by taking evading route
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
                Flanking(flankingSide);
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
            
            if (Flanking(flankingSide))
                return;

            if (Chasing())
                return;

            if (GoToGroupObjective())
                return;
        }

    }

    //Checks



    // Checks if the agent is in the objective area
    private bool InObjectiveArea()
    {
        return Vector3.Distance(transform.position, agent.AgentGroup.Objectives[0]) < inAreaThreshold;
    }

    // Positioning methods

    

    // Targetting methods

    // Go after closest agent that is closer than a threshold
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
        if (agent.TargetAgent != null && agent.seenOtherAgents.ContainsKey(agent.TargetAgent.Enemy.Name))
        {
            agent.Shoot();
        } else
        {
            agent.DontShoot();
        }
    }

    // --------------------------------------------------------------
    // --------------------------------------------------------------

    private bool GoToGroupObjective()
    {
        ((AgentBehaviour)this).GoToGroupObjective();

        StayInGroup();
        if (agent.TargetAgent != null)
        {
            Stop();

        }
        return positioning == PositioningMethod.GoToGroupObjective;

    }

    private bool Chasing()
    {
        ((AgentBehaviour)this).Chasing();

        if (agent.TargetAgent == null)
        {
            Stop();
            return false;
        }

        if (Vector3.Distance(transform.position, agent.TargetAgent.LastPosition) < flankThreshold)
        {
            flankingSide = ((AgentBehaviour)this).Flanking(flankingSide);
        }

        return positioning == PositioningMethod.Chasing;

    }

    private bool Flanking(int flankingSide)
    {
        flankingSide = ((AgentBehaviour)this).Flanking(flankingSide);
        if (agent.TargetAgent == null)
        {
            Stop();
            return false;
        }

        if (Vector3.Distance(transform.position, agent.TargetAgent.LastPosition) > flankThreshold)
        {
            Stop();
        }
        

        if (flankingSide == 0)
            Stop();

        return positioning == PositioningMethod.Flanking;
    }

}
