using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : AbstractProjectile {

    public GameObject Source { get; set; }

	// Use this for initialization
	void Start () {
	}

	protected override void onCollision (GameObject g) {
		AbstractProjectileCollidable c;
		if((c = g.GetComponent<AbstractProjectileCollidable>()) != null) {
			if (c.bulletCollides (dTrav*vel, dTrav, dTrav / travMax)) {
				c.receiveDamage (damage);
				Destroy (this.gameObject);
			}
		}

        if (g.tag == "Obstacles")
        {
            Debug.Log(g.tag);
            Destroy(this);
        }
	}
	protected override void bulletUpdate (float dTime) {
        dTrav += dTime;
		transform.Translate (dir * vel * dTime);
        if (dTrav >= travMax) {
			Destroy (this.gameObject);
		}
	}
}
