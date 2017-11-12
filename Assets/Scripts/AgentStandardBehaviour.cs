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
        if (agent.Target == null && agent.seenOtherAgents.Count == 0 && agent.Destination != transform.position)
        {
            agent.LookingDestination = agent.Destination;
        }
        if (agent.Target == null && agent.seenOtherAgents.Count > 0)
        {
            agent.Target = agent.seenOtherAgents[0];
            foreach (GameObject a in agent.seenOtherAgents)
            {
                if (Vector3.Distance(transform.position, a.transform.position) < Vector3.Distance(transform.position, agent.Target.transform.position))
                {
                    agent.Target = a;
                }
            }
        }
        if (agent.Target != null && !agent.seenOtherAgents.Contains(agent.Target))
        {
            agent.Target = null;
        }
        if (agent.Target != null)
        {
            agent.LookingDestination = agent.Target.transform.position;
        }

    }

    private void Chasing()
    {
        if (agent.Target != null)
        {
            agent.Destination = agent.Target.transform.position;
        } 
    }
}
