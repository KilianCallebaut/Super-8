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

    public static Agent spawnAgent( AgentAttributes attributes)
    {
        var agentSpawn = Instantiate(attributes.agentPrefab);
        Agent agentScript = agentSpawn.AddComponent<Agent>() as Agent;
     
        agentScript.Attributes = attributes;
        return agentScript;
    }
}
