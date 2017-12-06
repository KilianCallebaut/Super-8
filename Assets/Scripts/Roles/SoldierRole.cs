using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * Both attacker and defender, deals damage from medium distance with well placed shots
 * */
public class SoldierRole : AgentBehaviour {

    int flankingSide = 0;

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
    protected override void Inspecting()
    {
        if (agent.TargetAgent == null)
        {
            CheckFlanks();
        }

        if (agent.TargetAgent != null)
        {
            turningDestination = Vector3.zero;
            agent.LookingDestination = agent.TargetAgent.LastPosition;
        }
    }

    // Targets close flanking enemies and midrange enemies
    // Only meant to change the agents Target
    // goal: Deals with flanking enemies, picks best targets from cover position, deals with strongest targets first
    protected override void Targetting()
    {

         if (agent.seenOtherAgents.Count > 0)
        {

            Prioritizing();
        }
        Engaging();

    }



    // Keeps medium distance from targets
    // Since the only thing that is really meant to change here is the destination, we can order
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
            default:
                Stop();
                break;

        }



        if (positioning == PositioningMethod.Stop)
        {

            if (Retreat())
                return;

          

            if (Chasing())
                return;

            if (GoToGroupObjective())
                return;
        }

       
    }

    // Inspecting methods

    

    // Targetting methods

    // Decide when to shoot
    // For now always when agent has a target
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


    // Target midrange agents. If one oversteps a threshold, take him out. If one is flanking, take him out.
    private void Prioritizing()
    {
        foreach (KeyValuePair<string, OtherAgent> a in agent.AgentGroup.SharedSeenOtherAgents)
        {
            if (a.Value.Team != agent.Team && !agentIsTarget(a.Value))
            {
                if (agent.TargetAgent == null)
                {
                    agent.TargetAgent = new Target(a.Value, Time.time, 0);
                    continue;
                }

                if (agent.TargetAgent.ThreatLevel < 1 &&
                    Vector3.Distance(transform.position, a.Value.Position) < longRange)
                {
                    agent.TargetAgent = new Target(a.Value, Time.time, 1);
                    continue;
                }

                // When seeing someone flanking
                if (agent.TargetAgent.ThreatLevel < 2 &&
                    Vector3.Angle(agent.direction, (a.Value.Position - transform.position).normalized) > 45.0f )
                {
                    agent.TargetAgent = new Target(a.Value, Time.time, 2);
                    continue;
                }

                // When someone is super close
                if (agent.TargetAgent.ThreatLevel < 3
                    && closeRange > Vector3.Distance(transform.position, a.Value.Position) )
                {
                    agent.TargetAgent = new Target(a.Value, Time.time, 3);
                    continue;
                }

                
            }

            if (agentIsTarget(a.Value))
            {
                // Standared medium range 
                if (agent.TargetAgent.ThreatLevel < 1 &&
                    Vector3.Distance(transform.position, a.Value.Position) < longRange)
                {
                    agent.TargetAgent = new Target(a.Value, Time.time, 1);
                }

                // When seeing someone flanking
                if (agent.TargetAgent.ThreatLevel < 2 && Vector3.Angle(agent.direction, (a.Value.Position - transform.position).normalized) > 45.0f )
                {
                    agent.TargetAgent.ThreatLevel = 2;
                }

                // When someone is super close
                if (agent.TargetAgent.ThreatLevel < 3 && closeRange > Vector3.Distance(transform.position, a.Value.Position) )
                {
                    agent.TargetAgent.ThreatLevel = 3;
                }
            }
        }
    }

    // Positioning methods

    // Retreats to position further from target
    // For now go in exact opposite direction
    private bool Retreat()
    {
        ((AgentBehaviour)this).Retreat();

        if (agent.TargetAgent == null)
            Stop();

        if (!EnemiesNear())
            Stop();

        return positioning == PositioningMethod.Retreat;
    }

    private bool GoToGroupObjective()
    {
        ((AgentBehaviour)this).GoToGroupObjective();

        StayInGroup();
        if (agent.TargetAgent != null)
        {
            Stop();

        }

        if (EnemiesVeryNear())
            Stop();

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

        OtherAgent enemy = agent.seenOtherAgents.Where(x => x.Value.Team != agent.Team).FirstOrDefault().Value;
        if (enemy != null && Vector3.Distance(enemy.Position, transform.position) < longRange)
        {
            Stop();
        }


        return positioning == PositioningMethod.Chasing;

    }


    // For debugging
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
    }

}
