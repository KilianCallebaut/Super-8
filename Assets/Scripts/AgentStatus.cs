using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class AgentStatus {

	private static readonly ReadOnlyCollection<string> FIRST_NAMES = new ReadOnlyCollection<string>(new [] {"Sophie", "Angela", "Jacob", "Yvonne", "Karen", "Vanessa", "Amanda", "Benjamin", "Pippa", "Rachel", "Olivia", "Samantha", "Natalie", "Grace", "Emma", "Una", "Natalie", "Rose", "Stephanie", "Trevor"});
	private static readonly ReadOnlyCollection<string> SURNAMES = new ReadOnlyCollection<string>(new [] {"Stewart", "Reid", "Hart", "White", "Dowd", "Harris", "Nash", "Dyer", "Manning", "Allan", "Rampling", "Ogden", "Arnold", "Bond", "McDonald", "Rutherford", "Abraham", "Knox", "Coleman", "MacDonald"});

    // TODO FIX DROPDOWN
    public enum TrainingEnum
    {
        Gaming, Lifting, Drinking, Dancing
    }

	public string Name { get; private set; }
	public float Cost { get; private set; }
    public bool Active { get; set; }
	public bool FreeAgent { get; set; } // is the agent on the market? 
    public TrainingEnum Training = TrainingEnum.Dancing;
	public AgentAttributes Attributes { get; private set; }

	private int health; // 0-100

	public AgentStatus(bool freeAgent, bool active) {
        Attributes = new AgentAttributes(null); // TEMPORARY, TO BE CHANGED
		FreeAgent = freeAgent;
        Active = active;
		Name = FIRST_NAMES [Random.Range (0, FIRST_NAMES.Count)] + " " + SURNAMES [Random.Range (0, SURNAMES.Count)];
		Cost = 10.5f;
	}
}
