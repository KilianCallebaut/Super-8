using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyRole : AgentBehaviour {


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public override void Think()
    {
        if (agent == null)
            agent = gameObject.GetComponent<Agent>();

        Inspecting();
        Positioning();
    }

    protected override void Inspecting()
    {
        agent.LookingDestination = agent.Destination;
    }

    protected override void Positioning()
    {
        agent.Destination = new Vector3(30, 19);
    }

    protected override void Targetting()
    {
        throw new System.NotImplementedException();
    }
}
