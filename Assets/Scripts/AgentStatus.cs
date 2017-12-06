using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class AgentStatus {

	private static readonly ReadOnlyCollection<string> FIRST_NAMES = new ReadOnlyCollection<string>(new [] {"Sophie", "Angela", "Jacob", "Yvonne", "Karen", "Vanessa", "Amanda", "Benjamin", "Pippa", "Rachel", "Olivia", "Samantha", "Natalie", "Grace", "Emma", "Una", "Natalie", "Rose", "Stephanie", "Trevor"});
	private static readonly ReadOnlyCollection<string> SURNAMES = new ReadOnlyCollection<string>(new [] {"Stewart", "Reid", "Hart", "White", "Dowd", "Harris", "Nash", "Dyer", "Manning", "Allan", "Rampling", "Ogden", "Arnold", "Bond", "McDonald", "Rutherford", "Abraham", "Knox", "Coleman", "MacDonald"});

    public enum TrainingEnum
    {
        Gaming, Lifting, Drinking, Dancing
    }

    public enum RolesEnum
    {
        Assault, Heavy, Shadow, Sniper, Soldier
    }
    private static readonly ReadOnlyCollection<string> ROLE2STRING = new ReadOnlyCollection<string>(new[] { "Assault", "Heavy", "Shadow", "Sniper", "Soldier" });

    public enum GroupsEnum
    {
        Albert, Bob, Cooper, Diane, Ed, F, Gordon, Hawke
    }

	public string Name { get; private set; }
	public float Cost { get; private set; }

    private bool _active;
    public bool Active {
        get { return _active; }
        set
        {
            _active = value;
            teamManager.NotifyActiveChange(this);
        }
    }
	public bool FreeAgent { get; set; } // is the agent on the market? 

    public TrainingEnum Training = TrainingEnum.Dancing;
    public RolesEnum Role = RolesEnum.Shadow;
    public GroupsEnum Group = GroupsEnum.Albert;
	public AgentAttributes Attributes { get; private set; }
    private TeamManager teamManager;

	public AgentStatus(TeamManager teamManager, bool freeAgent, bool active) {
        this.teamManager = teamManager;
        Attributes = new AgentAttributes(null); // TEMPORARY, TO BE CHANGED
		FreeAgent = freeAgent;
        Active = active;
		Name = FIRST_NAMES [Random.Range (0, FIRST_NAMES.Count)] + " " + SURNAMES [Random.Range (0, SURNAMES.Count)];
		Cost = 10.5f;
	}

    public Agent CreateAgent(Group group, GameObject agentPrefab)
    {
        Agent newAgent = ObjectManager.spawnAgent(new AgentAttributes(Attributes, agentPrefab), ROLE2STRING[(int)Role]);
        newAgent.name = Name;

        return newAgent;
    }
}
