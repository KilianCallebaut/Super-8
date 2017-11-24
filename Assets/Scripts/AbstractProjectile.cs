using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractProjectile : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	protected Vector3 dir;
	public float vel = 15.0f;
	protected GameObject ignoreCollisionsWith = null;
	public float travMax = 20.0f;
	protected float dTrav = 0.0f;
	public int damage = 1;

	public void initialUpdate( GameObject doNotCollideWith, Vector3 direction) {
		dir.x = direction.x;
		dir.y = direction.y;
		dir.z = 0.0f;
		ignoreCollisionsWith = doNotCollideWith;
		bulletUpdate (Time.deltaTime);
	}

	protected abstract void onCollision (GameObject g);
	protected abstract void bulletUpdate (float dTime);

	void OnTriggerEnter2D(Collider2D c) {
		if (c.gameObject == ignoreCollisionsWith || c.gameObject.GetComponent<AbstractProjectileCollidable>() == null)
			return;
		onCollision (c.gameObject);
	}

	// Update is called once per frame
	void Update () {
		bulletUpdate (UnityEngine.Time.deltaTime);
	}
}
