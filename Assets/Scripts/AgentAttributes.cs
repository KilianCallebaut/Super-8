using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAttributes {


    // 2.0 to 5.0
    // Speed of agent's movement
    public float speed { get;  private set; }

    // 10.0 to 20.0
    // Reach of agent's vision
    public float reachOfVision { get; private set; }

    // 22.5
    // Width of agent's vision
    public float widthOfVision { get;  private set;}

    // Time between spotting an agent and shooting
    public float reflex { get; private set; }

    // Rotationspeed
    public float agility  { get;  private set;}

    // aiming offset's angle
    public float accuracy  { get;  private set;}

    // Maximum damage agent can take
    public float maxHealth { get; private set; }

    // Determines reduction by heavy gear
    public float strength { get; private set; }

    public GameObject agentPrefab { get; private set; }


    // Personalized agentattributes
    public AgentAttributes(float speed, float reachOfVision, float widthOfVision,
        float reflex, float agility, float accuracy, float maxHealth, 
        GameObject agentPrefab)
    {
        this.speed = speed;
        this.reachOfVision = reachOfVision;
        this.widthOfVision = widthOfVision;
        this.reflex = reflex;
        this.agility = agility;
        this.accuracy = accuracy;
        this.maxHealth = maxHealth;
        this.agentPrefab = agentPrefab;

    }

    // Default/debugger agentattributes
    public AgentAttributes(GameObject agentPrefab)
    {
        this.speed = 2.0f;
        this.reachOfVision = 20.0f;
        this.widthOfVision = 22.5f;

      
        this.reflex = 1.0f / 2.0f;
        this.agility = 1.0f;
        this.accuracy = 10.0f;
        this.maxHealth = 20.0f;
        this.agentPrefab = agentPrefab;
    }


    public void standardHeavy()
    {

    }

}
