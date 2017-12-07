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

    // Bonusses & boosts
    private float speedbuff = 1.0f;
    public void speedProduct(float s)
    {
        speed = speed / speedbuff;

        if (s == 0.0f)
        {
            speedbuff = 1.0f;
        } else
        {
            speedbuff = s;
            speed = speed * speedbuff;
        }
    }

    private float agilitybuff = 1.0f;
    public void agilityProduct(float s)
    {
        agility = agility / agilitybuff;

        if (s == 0.0f)
        {
            agilitybuff = 1.0f;
        }
        else
        {
            agilitybuff = s;
            agility = agility * speedbuff;
        }
    }

    // Gear
    public void healthGear(float h)
    {
        this.maxHealth += h;
    }

    public void accuracyGear(float a)
    {
        this.accuracy += a;
    }

    public void heavyGear(float h)
    {
        this.speed -= h;
    }

    public void scopeGear(float s)
    {
        this.reachOfVision += s;
    }

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

    public AgentAttributes(AgentAttributes attributes, GameObject agentPrefab)
    {
        this.speed = attributes.speed;
        this.reachOfVision = attributes.reachOfVision;
        this.widthOfVision = attributes.widthOfVision;
        this.reflex = attributes.reflex;
        this.agility = attributes.agility;
        this.accuracy = attributes.accuracy;
        this.maxHealth = attributes.maxHealth;
        this.agentPrefab = agentPrefab;

    }

    // Default/debugger agentattributes
    public AgentAttributes(GameObject agentPrefab)
    {
        this.speed = 2.0f;
        this.reachOfVision = 23.0f;
        this.widthOfVision = 45f;
        
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
