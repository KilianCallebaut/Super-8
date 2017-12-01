using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStatus : MonoBehaviour {
	private AgentAttributes attributes;
	private int health; // 0-100
	private bool active = false;
	private GameObject playerBox;
	private PlayerBoxController playerBoxController;
	private TeamManager teamManager;

	public AgentStatus(TeamManager teamManager, bool active) {
		this.teamManager = teamManager;
		this.playerBox = Instantiate(teamManager.PlayerBoxPrefab);
		this.playerBoxController = this.playerBox.gameObject.GetComponent<PlayerBoxController>();
		if (active) Activate();
		else Deactivate();
	}

	public void Activate() {
		active = true;
		teamManager.ActivateAgent (this);
	}

	public void Deactivate() {

	}

	public void SetParent(Transform parent) {
		playerBox.transform.parent = parent;
	}

}
