using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverScript : AbstractProjectileCollidable {

	// Use this for initialization
	void Start () {
		
	}
	public int maxDamage = 10;
	public int damageReceived = 0;
	public float cover = 0.3f;
	public override bool collides (float dist, float time, float frac) {
		float f = Random.value;
		f = (1.0f - frac) * f + frac;
		return f < (1.0f - cover);
	}
	public override void receiveDamage (int damage) {
		damageReceived += damage;
		if (damageReceived >= maxDamage) {
			Destroy (this.gameObject);
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
