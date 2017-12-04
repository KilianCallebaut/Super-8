using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperRole : AgentBehaviour {
  
    

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
            Engaging();
        }
    }

    // Set up from a distance
    protected override void Positioning()
    {
       

     

        if (agent.AtDestination())
        {
            GoToGroupObjective();
            StayInGroup();
            
        }

        if (InObjectiveArea() && agent.InFieldOfVision(agent.Destination))
        {
            agent.Destination = transform.position;
        }


        if (EnemiesAlmostNear())
        {
            Retreat();
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
}
