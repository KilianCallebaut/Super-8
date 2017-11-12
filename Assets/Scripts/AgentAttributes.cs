using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAttributes {


    public float speed { get;  private set; }
    public float reachOfVision { get; private set; }
    public float widthOfVision { get;  private set;}
    public float reflex { get; private set; }
    public float agility  { get;  private set;}
    public float accuracy  { get;  private set;}

    // Personalized agentattributes
    public AgentAttributes(float speed, float reachOfVision, float widthOfVision,
        float reflex, float agility, float accuracy)
    {
        this.speed = speed;
        this.reachOfVision = reachOfVision;
        this.widthOfVision = widthOfVision;
        this.reflex = reflex;
        this.agility = agility;
        this.accuracy = accuracy;

    }

    // Default/debugger agentattributes
    public AgentAttributes()
    {
        this.speed = 2.0f;
        this.reachOfVision = 5.0f;
        this.widthOfVision = 50.0f;
        this.reflex = 1.0f / 2.0f;
        this.agility = 2.0f;
        this.accuracy = 10.0f;
    }

}
