using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    abstract public void Think();
}
