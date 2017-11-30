using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowRole : AgentBehaviour {

    private int flankingSide = 0;

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
        Targetting();
        Positioning();
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
        GoToGroupObjective();
        StayInGroup();


        if (agent.TargetAgent != null)
        {
            // When targets looks away from you, sneak up on him, else avoid his gaze
            if (!InEnemyBack(agent.TargetAgent.Enemy))
            {
                Chasing();
            }
            else
            {
                HittingTheBack();
            }
        }
        
    }

    // Prefers targets further away
    protected override void Targetting()
    {
        if (agent.seenOtherAgents.Count > 0)
        {
            if (agent.TargetAgent == null) 
                Prioritizing();

            Engaging();
        }
    }

    // Go after closest agent that is closer than a threshold
    private void Prioritizing()
    {
        foreach (KeyValuePair<string, OtherAgent> a in agent.seenOtherAgents)
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
        if (agent.TargetAgent != null)
        {
            agent.Shoot();
        }
    }
}
