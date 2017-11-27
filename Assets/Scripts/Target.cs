using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target  {

    public OtherAgent Enemy { get; private set; }
    public float AimTime { get; set; }

    public Vector3 LastPosition
    {
        get
        {
            return Enemy.Position;
        }
    }

    public Target(OtherAgent agent, float aimTime)
    {
        this.Enemy = agent;
        this.AimTime = aimTime;
    }
}
