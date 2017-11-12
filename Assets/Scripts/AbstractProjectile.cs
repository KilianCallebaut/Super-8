using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractProjectile : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	protected Vector3 dir;
	protected float vel;
	protected GameObject ignoreCollisionsWith = null;


	public void initialUpdate(float dTime, GameObject doNotCollideWith, Vector3 direction) {
		dir.x = direction.x;
		dir.y = direction.y;
		ignoreCollisionsWith = doNotCollideWith;
		transform.Translate (dir * vel * dTime);
	}

	// Update is called once per frame
	void Update () {
		transform.Translate (dir * vel * UnityEngine.Time.deltaTime);
	}
}
