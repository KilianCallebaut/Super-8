using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target  {

    public OtherAgent Enemy { get; private set; }
    public float AimTime { get; set; }
    public bool Seen { get; set; }

    public Vector3 LastPosition
    {
        get
        {
            return Enemy.Position;
        }
    }

    public float AimOffset
    {
        get
        {
            if (Seen)
            {
                float offset = Time.time - AimTime;
                return offset;
            }
            AimTime = Time.time;
            return 0.0f;
        }
    }

    public Target(OtherAgent agent, float aimTime)
    {
        this.Enemy = agent;
        this.AimTime = aimTime;
        this.Seen = false;
    }
}
