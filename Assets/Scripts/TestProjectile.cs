using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProjectile : AbstractProjectile {

	// Use this for initialization
	void Start () {
		
	}
	private override void onCollision(GameObject g) {
		Destroy (this.gameObject);
	}
	private override void bulletUpdate(float dTime) {
		transform.Translate (dir * vel * dTime);
	}
	// Update is called once per frame
	void Update () {
		
	}
}
