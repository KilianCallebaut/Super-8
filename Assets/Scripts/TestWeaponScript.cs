using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWeaponScript : MonoBehaviour {

	public Vector3 shootDir = new Vector3(1.0f, 0.0f);

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<Weapon> ().setShooting (true);
		gameObject.GetComponent<Weapon> ().setShootingDirection(shootDir);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
