using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target  {

    public GameObject enemy { get; private set; }
    public float aimTime { get; set; }

    public Vector3 position
    {
        get
        {
            return enemy.transform.position;
        }
    }

    public float aimingBonus
    {
        get
        {
            float offset = Time.time - aimTime;
            return offset;
        }
    }

    public Target(GameObject agent, float aimTime)
    {
        this.enemy = agent;
        this.aimTime = aimTime;
    }
}
