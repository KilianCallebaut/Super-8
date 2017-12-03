using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class TeamManager : MonoBehaviour {

    public PlayersScreenController PlayersScreen;

    private List<AgentStatus> agents;

	// Use this for initialization
	void Start () {
        agents = new List<AgentStatus>();

        // this shit is temporary
        for (int i = 0; i < 8; ++i)
        {
            agents.Add(new AgentStatus(this, true));
        }
        for (int i = 0; i < 2; ++i)
        {
            agents.Add(new AgentStatus(this, false));
        }

        PlayersScreen.Initialize(this);
    }

    public ReadOnlyCollection<AgentStatus> ReadAgents()
    {
        return agents.AsReadOnly();
    }

    public void AddAgent(AgentStatus agent, bool updatePlayersScreen)
    {
        agents.Add(agent);

        if (updatePlayersScreen) PlayersScreen.AddPlayerBox(agent, false);
    }

    public void RemoveAgent(AgentStatus agent, bool updatePlayersScreen)
    {
        agents.Remove(agent);

        if (updatePlayersScreen) PlayersScreen.RemovePlayerBox(agent, false);
    }


}
