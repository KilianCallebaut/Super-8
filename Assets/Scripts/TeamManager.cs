using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour {

    public GameObject PlayersScreen;
    public GameObject PlayerBoxPrefab;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < 8; ++i)
        {
			new AgentStatus (this, true);
        }
	}

	public void ActivateAgent(AgentStatus agent) {
		var activeGrid = PlayersScreen.transform.Find ("ActiveGrid");
		agent.SetParent (activeGrid);
	}
}
