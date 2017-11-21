using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Both attacker and defender, deals damage from medium distance with well placed shots
 * */
public class SoldierRole : AgentBehaviour {

    private Agent agent;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
    // goal: Checks for flanking every once in a while, mostly focused on objective
    private void Inspecting()
    {
        if (agent.TargetAgent == null)
        {
            LookAround();
        }

        if (agent.TargetAgent != null)
        {
            agent.LookingDestination = agent.TargetAgent.LastPosition;
        }
    }

    // Targets close flanking enemies and midrange enemies
    // Only meant to change the agents Target
    // goal: Deals with flanking enemies, picks best targets from cover position, deals with strongest targets first
    private void Targetting()
    {
        
    }


    // Keeps medium distance from targets
    // Since the only thing that is really meant to change here is the destination, we can order
    private void Positioning()
    {

    }

    // Inspecting methods

    // Look around in an angle of the current direction
    // For now look at destination 
    private void LookAround()
    {
        agent.LookingDestination = Quaternion.Euler(0, 0, agent.Attributes.widthOfVision) * agent.visionDirection * agent.Attributes.reachOfVision + transform.position;

    }

    // Targetting methods
}
