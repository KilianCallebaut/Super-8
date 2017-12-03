using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStatus {

    // TODO FIX DROPDOWN
    public enum TrainingEnum
    {
        Gaming, Lifting, Drinking, Dancing
    }

    public bool Active { get; set; }
    public TrainingEnum Training = TrainingEnum.Dancing;

	public AgentAttributes Attributes { get; private set; }
	private int health; // 0-100
	private GameObject playerBox;
	private TeamManager teamManager;

	public AgentStatus(TeamManager teamManager, bool active) {
        Attributes = new AgentAttributes(null); // TEMPORARY, TO BE CHANGED
		this.teamManager = teamManager;
        Active = active;
	}
}
