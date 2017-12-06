using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TeamManager : MonoBehaviour {

	public const int MAX_TEAM_SIZE = 12;

    public PlayersScreenController PlayersScreen;
    public PlayScreenController PlayScreen;
	public Text TeamNameValue;
	public Text CostValue;
	public Text NoPlayersValue;

	private string _name;
	public string Name { 
		get { return _name; } 
		private set { 
			_name = value;
			TeamNameValue.text = value;
		}
	}
	private float _money;
	public float Money {
		get { return _money; }
		private set {
			_money = value;
			CostValue.text = value.ToString("0.0");
		} 
	}
    private List<AgentStatus> agents;

	// Use this for initialization
	void Start () {
		Name = "Cool Kill";
		Money = 1000f;
        agents = new List<AgentStatus>();

        // this shit is temporary
        for (int i = 0; i < 8; ++i)
        {
            agents.Add(new AgentStatus(this, false, true));
        }
        for (int i = 0; i < 2; ++i)
        {
            agents.Add(new AgentStatus(this, false, false));
        }

		NoPlayersValue.text = agents.Count.ToString();

        PlayersScreen.Initialize(this);
        PlayScreen.Initialize(this);
    }

    public ReadOnlyCollection<AgentStatus> ReadAgents()
    {
        return agents.AsReadOnly();
    }

	public bool CanBuyAgent(AgentStatus agent) {
		return Money >= agent.Cost && agents.Count < MAX_TEAM_SIZE;
	}

	public void BuyAgent(AgentStatus agent) {
		agent.FreeAgent = false;
		Money -= agent.Cost;
		AddAgent (agent, true);
	}

    public void AddAgent(AgentStatus agent, bool updatePlayersScreen)
    {
		if (agents.Count >= MAX_TEAM_SIZE)
			return;

		if (agent.Active && agents.Count (tempAgent => tempAgent.Active) >= 8) {
			agent.Active = false;
		}

		if (!agent.Active && agents.Count (tempAgent => !tempAgent.Active) >= 4) {
			agent.Active = true;
		}
			
		
        agents.Add(agent);
		NoPlayersValue.text = agents.Count.ToString();

        if (updatePlayersScreen) PlayersScreen.AddPlayerBox(agent, false);
        PlayScreen.RenderPlayers();
    }

    public void RemoveAgent(AgentStatus agent, bool updatePlayersScreen)
    {
        agents.Remove(agent);
		NoPlayersValue.text = agents.Count.ToString();

        if (updatePlayersScreen) PlayersScreen.RemovePlayerBox(agent, false);
        PlayScreen.RenderPlayers();
    }

    // god I wish there was easy way to events
    public void NotifyActiveChange(AgentStatus agent)
    {
        PlayScreen.RenderPlayers();
    }


}
