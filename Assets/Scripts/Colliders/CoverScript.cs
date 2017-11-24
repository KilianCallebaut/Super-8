using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverScript : AbstractProjectileCollidable {

	// Use this for initialization
	void Start () {
		
	}
	public int maxDamage = 10;
	public int damageReceived = 0;
	public bool destroyable = true;
	public override bool bulletCollides (float dist, float time, float frac) {
		
		float f = Random.value;
		f = f * hitChanceMultiplier;//higher HCM value means higher chance to hit. Lower means less chance.
		return f > (1.0f - frac);
	}
	public override void receiveDamage (int damage) {
		Debug.Log (this.gameObject.name + " took " + damage + " points of damage.");
		damageReceived += damage;
		if (damageReceived >= maxDamage && destroyable) {
			Destroy (this.gameObject);
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
