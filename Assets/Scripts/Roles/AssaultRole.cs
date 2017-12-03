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
    private float flankThreshold = 7.5f;
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
        
    }

    // Looks around when no target is selected
    // Only meant to change the LookingDestination
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





    // Targets closest enemy under distancethreshold
    // Only meant to change the agents Target
    protected override void Targetting()
    {
       
        if (agent.seenOtherAgents.Count > 0)
        {
            Prioritizing();
        }

        Engaging();

    }

    // When spotting an agent that's close enough approach otherwise avoid
    // Since the only thing that is really meant to change here is the destination, we can order
    // Goal: Avoiding line of fire by taking evading route
    protected override void Positioning()
    {
        if (agent.TargetAgent != null)
        {
            Stop();
        }

        if (agent.AtDestination())
        {
            GoToGroupObjective();
            StayInGroup();

            Chasing();

            if (Vector3.Distance(transform.position, agent.TargetAgent.LastPosition) < flankThreshold)
            {
                flankingSide = Flanking(flankingSide);
            }
            
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
        if (agent.TargetAgent != null)
        {
            agent.Shoot();
        }
    }



   
}
