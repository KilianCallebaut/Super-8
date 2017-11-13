using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStandardBehaviour : AgentBehaviour {

    private Agent agent;

	// Use this for initialization
	void Start () {
        agent = GetComponent<Agent>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Think()
    {

        // Placeholder for pathfinding
        if (agent.Destination == transform.position || agent.Destination == Vector3.zero)
        {
            MoveToRandomDestination();
        }

        Targetting();
        Chasing();
    }

    // Decisions
    // Placeholder for pathfinding
    private void MoveToRandomDestination()
    {
        int index = Random.Range(0, LevelManager.Instance.Tiles.Count);

        GameObject tile = (GameObject)LevelManager.Instance.Tiles[index];
        agent.Destination = tile.transform.position;
    }

    // Placeholder for Targetting
    private void Targetting()
    {
        if (agent.TargetAgent == null && agent.seenOtherAgents.Count == 0 && agent.Destination != transform.position)
        {
            agent.LookingDestination = agent.Destination;
        }
        if (agent.TargetAgent == null && agent.seenOtherAgents.Count > 0)
        {
            agent.TargetAgent = new Target(agent.seenOtherAgents[0], Time.time);
            foreach (GameObject a in agent.seenOtherAgents)
            {
                if (Vector3.Distance(transform.position, a.transform.position) < Vector3.Distance(transform.position, agent.TargetAgent.position))
                {
                    agent.TargetAgent = new Target(a, Time.time);
                }
            }
        }
        if (agent.TargetAgent != null && !agent.seenOtherAgents.Contains(agent.TargetAgent.enemy))
        {
            agent.TargetAgent = null;
        }
        if (agent.TargetAgent != null)
        {
            agent.LookingDestination = agent.TargetAgent.position;
        }

    }

    // Chasing placeholder
    private void Chasing()
    {
        if (agent.TargetAgent != null)
        {
            agent.Destination = agent.TargetAgent.position;
        } 
    }
}
