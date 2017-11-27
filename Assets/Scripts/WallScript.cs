﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallScript : AbstractProjectileCollidable {

	// Use this for initialization
	void Start () {
		
	}
	public abstract bool bulletCollides(float dist, float time, float frac){
		return true;
	}
	public abstract void receiveDamage(int damage) {
		return;//walls don't break
	}
	// Update is called once per frame
	void Update () {
		
	}
}
