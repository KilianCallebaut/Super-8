using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProjectile : AbstractProjectile {

	// Use this for initialization
	void Start () {
		
	}

	protected override void onCollision(GameObject g) {
		AbstractProjectileCollidable a = g.GetComponent<AbstractProjectileCollidable> ();
		if (a.bulletCollides (dTrav*vel, dTrav, dTrav / travMax)) {
			a.receiveDamage (damage);
			Destroy (this.gameObject);
		}
	}
	protected override void bulletUpdate(float dTime) {
		dTrav += dTime;
		transform.Translate (dir * vel * dTime);
		if (dTrav >= travMax) {
			Destroy (this.gameObject);
		}
	}
}
