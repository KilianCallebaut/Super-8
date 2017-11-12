using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour {

 

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static GameObject spawnAgent(GameObject agent, AgentAttributes attributes)
    {
        var agentSpawn = Instantiate(agent);
        Agent agentScript = agentSpawn.AddComponent<Agent>() as Agent;
     
        agentScript.Attributes = attributes;
        return agentSpawn;
    }
}
