using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractProjectile : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	protected Vector3 dir;
	protected float vel = 1.0f;
	protected GameObject ignoreCollisionsWith = null;


	public void initialUpdate(float dTime, GameObject doNotCollideWith, Vector3 direction) {
		dir.x = direction.x;
		dir.y = direction.y;
		dir.z = 0.0f;
		ignoreCollisionsWith = doNotCollideWith;
		bulletUpdate (dTime);
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
