using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageRecipient : AbstractProjectileCollidable {

	// Use this for initialization
	void Start () {
		
	}

	public int maxDamage = 10;
	public int damageReceived = 0;
	public override bool bulletCollides (float dist, float time, float frac) {
		return true;//I think it might be frustrating to see a character dodge bullets?
	}
	public override void receiveDamage (int damage) {
		Debug.Log (this.gameObject.name + " took " + damage + " points of damage.");
		//take damage
		damageReceived += damage;
		//maybe look up ANOTHER script and give it the signal that it's time to take damage?
	}

	// Update is called once per frame
	void Update () {
		
	}
}
