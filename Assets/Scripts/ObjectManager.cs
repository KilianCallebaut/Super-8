using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : Singleton<ObjectManager> {

    [SerializeField]
    private Rigidbody2D[] Bullets;

    

    //Returns Bullet of this type
    public Rigidbody2D getBulletOfType(int type)
    {
        return Bullets[type];
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void setRole(Agent agent, string role)
    {
        switch (role)
        {
            case "Heavy":
                agent.Behaviour = agent.gameObject.AddComponent<HeavyRole>();
                break;
            case "Assault":
                agent.Behaviour = agent.gameObject.AddComponent<AssaultRole>();
                break;
            default:
                agent.Behaviour = agent.gameObject.AddComponent<AgentStandardBehaviour>();
                break;
        }
    }

    public static Agent spawnAgent( AgentAttributes attributes, string role)
    {
        var agentSpawn = Instantiate(attributes.agentPrefab);
        Agent agentScript = agentSpawn.AddComponent<Agent>() as Agent;
        setRole(agentScript, role);
        agentScript.Attributes = attributes;
        return agentScript;
    }

 
}
