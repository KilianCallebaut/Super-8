using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProjectile : AbstractProjectile {

	// Use this for initialization
	void Start () {
		
	}
	public float travMax = 20.0f;
	public float dTrav = 0.0f;
	public int damage = 1;
	protected override void onCollision(GameObject g) {
		AbstractProjectileCollidable a = g.GetComponent<AbstractProjectileCollidable> ();
		if (a.collides (dTrav*vel, dTrav, dTrav / travMax)) {
			a.receiveDamage (damage);
		}
		Destroy (this.gameObject);
	}
	protected override void bulletUpdate(float dTime) {
		dTrav += dTime;
		transform.Translate (dir * vel * dTime);
	}
}
