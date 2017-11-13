using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//placeholder class to put on things you want to be able to fire from a gun
public class NullProjectile : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//do nothing
	}
	/*
	 * We are the projectiles who don't do anything
	 * we just spawn and lay around
	 * And if you ask us to do anything
	 * We'll just tell you we don't do anything
	 */
	private override void onCollision(GameObject g) {

	}
	private override void bulletUpdate(float dTime) {

	}

	// Update is called once per frame
	void Update () {
		//do nothing
	}
}
