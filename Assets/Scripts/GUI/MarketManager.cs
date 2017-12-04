using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketManager : MonoBehaviour {

	public MarketTableController MarketTable;
	public TeamManager TeamManagerRef;

	private List<AgentStatus> freeAgents;

	void Start () {
		freeAgents = new List<AgentStatus> ();

		for (int i = 0; i < 20; i++) {
			freeAgents.Add (new AgentStatus (true, true));
		}

		MarketTable.AddFreeAgents (freeAgents);
	}

	public void BuyAgent(AgentStatus agent) {
		freeAgents.Remove (agent);
		TeamManagerRef.BuyAgent (agent);
	}
}
