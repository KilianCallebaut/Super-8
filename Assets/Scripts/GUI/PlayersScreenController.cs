using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class PlayersScreenController : MonoBehaviour {

    public GameObject PlayerBoxPrefab;

    private TeamManager teamManager;
    private Transform activeGrid;
    private Transform deactiveGrid;
    private Dictionary<AgentStatus, PlayerBoxController> agent2playerBox;

    public void Initialize(TeamManager teamManager)
    {
        activeGrid = transform.Find("ActiveGrid");
        deactiveGrid = transform.Find("DeactiveGrid");
        agent2playerBox = new Dictionary<AgentStatus, PlayerBoxController>();

        this.teamManager = teamManager;
        foreach (AgentStatus agent in teamManager.ReadAgents())
        {
            AddPlayerBox(agent, false);
        }
    }

    public void AddPlayerBox(AgentStatus agent, bool updateTeamManager)
    {
        GameObject playerBox = Instantiate(PlayerBoxPrefab);
        PlayerBoxController playerBoxController = playerBox.gameObject.GetComponent<PlayerBoxController>();

		if (agent.Active) ActivateAgent(playerBoxController, agent);
		else if (!agent.Active) DeactivateAgent(playerBoxController, agent);

        playerBoxController.Initialize(this, agent.Attributes, agent);

        agent2playerBox.Add(agent, playerBoxController);
    }

    public void RemovePlayerBox(AgentStatus agent, bool updateTeamManager)
    {
        if (updateTeamManager) teamManager.RemoveAgent(agent, false);
        agent2playerBox[agent].Destroy();
    }

    public void ActivateAgent(PlayerBoxController playerBox, AgentStatus agent)
    {
        if (activeGrid.childCount >= 8) return;
        playerBox.SetParent(activeGrid);
        agent.Active = true;
    }

    public void DeactivateAgent(PlayerBoxController playerBox, AgentStatus agent)
    {
        playerBox.SetParent(deactiveGrid);
        agent.Active = false;
    }
}
