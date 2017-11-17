using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Group : MonoBehaviour
{
    
    public List<Vector3> Objectives { get; set; }
    public List<Vector3> SubObjectives { get; set; }
    public float Closeness { get; set; }
    public Dictionary<string, Agent> Members { get; set;}
    public Agent Leader { get; set; }
    public Dictionary<string, OtherAgent> SharedSeenOtherAgents { get; set; }
    public List<OtherAgent> TimesSeenOtherAgents { get; set; }
    private bool initialized = false;

	// Use this for initialization
	void Start () {
        Initialize();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Initialize()
    {
        if (!initialized)
        {
            Objectives = new List<Vector3>();
            SubObjectives = new List<Vector3>();
            Closeness = 6.0f;
            Members = new Dictionary<string, Agent>();
            SharedSeenOtherAgents = new Dictionary<string, OtherAgent>();
            TimesSeenOtherAgents = new List<OtherAgent>();
        }
    }


    // Event where agent shares a seen agent
    public void SeenAgent(OtherAgent oa)
    {
        Initialize();
        TimesSeenOtherAgents.Add(oa);
        SharedSeenOtherAgents.Add(oa.Name, oa);
    }

    // Event where agent no longer sees agent
    public void UnseenAgent(OtherAgent oa)
    {
        Initialize();

        int index = TimesSeenOtherAgents.FindLastIndex(x => x == oa);
        if (index != -1)
        {
            TimesSeenOtherAgents.RemoveAt(index);
        }
        if (!TimesSeenOtherAgents.Contains(oa))
        {
            SharedSeenOtherAgents.Remove(oa.Name);
        }

    }

    // Adds member and a leader if there's none
    public void AddMember(Agent a)
    {
        Initialize();

        a.AgentGroup = this;
        Members.Add(a.name, a);
        if ( Leader == null)
        {

            int index = UnityEngine.Random.Range(0, Members.Count);
            Leader = System.Linq.Enumerable.ToList(Members.Values)[index];
        }
    }

    // Adds leader
    public void AddLeader(Agent a)
    {
        Initialize();

        Leader = a;
        AddMember(a);
    }
}
