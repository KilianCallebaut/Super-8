using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class LevelManager : Singleton<LevelManager> {

    [SerializeField]
    private GameObject agent;
    

    [SerializeField]
    private GameObject group;

    public List<GameObject> Tiles { get; private set; }
    public List<Vector2> StartTilesTeam1 { get; private set; }
    public List<Vector2> StartTilesTeam2 { get; private set; }

    public List<Agent> Agents { get; private set; }
    public List<GameObject> Bullets { get; private set; } 
    public List<Vector3> Objectives { get; private set; }

	private GameObject tile_dump;
	private GameObject team1_parent;
	private GameObject team2_parent;
    private int numberOfMembers = 1;

	private Vector3 worldStart;

    private int[,] map;



    public int MaxXTiles
    {
        get
        {
            return map.GetLength(0);
        }
    }

    public int MaxYTiles
    {
        get
        {
            return map.GetLength(1);
        }
    }

  


    // Use this for initialization
    void Start () {
        initialize();
        Vector3 worldStart = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height));

    }

    // Update is called once per frame
    void Update () {
	
	}
		


    // For initialization
    private void initialize() {
        //readMap();

		// To clean up the overview window
		tile_dump = new GameObject ();
		tile_dump.name = "Tiles_and_Walls";
		team1_parent = new GameObject ();
		team1_parent.name = "Team_1";
		team2_parent = new GameObject ();
		team2_parent.name = "Team_2";

        Tiles = new List<GameObject>();
        StartTilesTeam1 = new List<Vector2>();
        StartTilesTeam2 = new List<Vector2>();

        Agents = new List<Agent>();

		GetStartTiles();
        Objectives = new List<Vector3>();
    }

    // What happens when you press the start button
    public void Go()
    {
        Vector3 worldStart = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height));
        CreateAgents(worldStart);
    }



    private void GetStartTiles()
    {
        Node[,] grid = Grid.Instance.getGrid();
        if (grid == null) {
            Grid.Instance.Instantiate();
            grid = Grid.Instance.getGrid();
        }
        var startT1 = getStartTilesTeam(true, grid);
        var startT2 = getStartTilesTeam(false, grid);
        

        for( int x = 0; x < startT1.Count; x++)
        {
            for (int y = 0; y< startT1[0].Count; y++)
            {
                Node n = startT1[x][y];
                if (n.walkable)
                {
                    StartTilesTeam1.Add(n.worldPosition);
                    y += 4;
                    if (y >= startT1[0].Count)
                        break;
                } 

            }
            x += 5;
            if (x >= startT1.Count)
                break;
        }

        for (int x = 0; x < startT2.Count; x++)
        {
            for (int y = 0; y < startT2[0].Count; y++)
            {
                Node n = startT2[x][y];
                if (n.walkable)
                {
                    StartTilesTeam2.Add(n.worldPosition);
                    y += 4;
                    if (y >= startT2[0].Count)
                        break;
                }

            }
            x += 5;
            if (x >= startT2.Count)
                break;
        }


    }

    private List<List<Node>> getStartTilesTeam(bool team1, Node[,] grid)
    {
        int tek = team1 ? 1 : -1;
        List<List<Node>> res = new List<List<Node>>();

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            List<Node> xrow = new List<Node>();
            for (int y = 0; y< grid.GetLength(1); y++)
            {
                Node n = grid[x, y];

                if (34 > tek*n.worldPosition.x && tek*n.worldPosition.x > 30 && Mathf.Abs(n.worldPosition.y) < 3)
                {
                    xrow.Add(n);
                }
               
            }
            if (xrow.Count>0)
                res.Add(xrow);
        }
        return res;

    }



    // Placeholder for agent spawning
    private void CreateAgents(Vector3 worldStart)
    {
        GameObject groupObj1 = Instantiate(group);
        Group group1 = groupObj1.AddComponent<Group>();
		group1.Initialize ();
        group1.Objectives = Objectives;
        group1.name = "Group1";
        for (int i = 0; i< numberOfMembers ; i++) //
        {
            Agent newAgent;
            // FOR DEBUGGING PURPOSES
            if (i < 2)
            {
                newAgent = ObjectManager.spawnAgent(new AgentAttributes(agent), "Soldier");
            } else
            {
                newAgent = ObjectManager.spawnAgent(new AgentAttributes(agent), "Shadow");
            }

            newAgent.name = "Agent_" + i + "_Team1";
            newAgent.transform.position = StartTilesTeam1[i];
            newAgent.Team = 1;
            Agents.Add(newAgent);
			newAgent.transform.SetParent (team1_parent.transform);
            group1.AddMember(newAgent);
        }

        GameObject groupObj2 = Instantiate(group);
        Group group2 = groupObj2.AddComponent<Group>();
		group2.Initialize ();
        group2.name = "Group2";

        // FOR DEBUGGING
        group2.Objectives = (new List<Vector3>());
        group2.Objectives.Add(Vector3.zero); 

        for (int i = 0; i < numberOfMembers ; i++) //
        {

            Agent newAgent = ObjectManager.spawnAgent(new AgentAttributes(agent), "Assault");
            newAgent.name = "Agent_" + i + "_Team2";
            newAgent.transform.position = StartTilesTeam2[i];
            newAgent.Team = 2;
            Agents.Add(newAgent);
			newAgent.transform.SetParent (team2_parent.transform);
            group2.AddMember(newAgent);
        }
    }

    // Placeholder for agent death
    public void DeleteAgent(Agent deletedAgent)
    {
        Agents.Remove(deletedAgent);

        foreach(Agent a in Agents)
        {
            if (a.seenOtherAgents.ContainsKey(deletedAgent.name))
            {
                a.seenOtherAgents.Remove(deletedAgent.name);
            }

            if (a.TargetAgent != null &&  a.TargetAgent.Enemy.Name == deletedAgent.name)
            {
                a.TargetAgent = null;
            }
        }

        Destroy(deletedAgent.gameObject);
        
    }

    // Temporary method for adding objective for 1 team
    public void AddObjectiveForTeam1(Vector3 Objective)
    {
        Objectives.Add(Objective);
    }

  
    public bool CanSee(Agent seeer, string name )
    {
        try
        {
            Agent a = LevelManager.Instance.Agents.Find(x => x.name == name);

           

            Vector3 objectPosition = a.transform.position;
            Vector3 seeerPosition = seeer.transform.position;


            float distanceToObject = (objectPosition - seeerPosition).magnitude;
            Vector3 directionToObject = (objectPosition - seeerPosition) / distanceToObject;

            if (distanceToObject < seeer.Attributes.reachOfVision && Vector3.Angle(directionToObject, seeer.visionDirection) < seeer.Attributes.widthOfVision
                && Vector3.Angle(directionToObject, seeer.visionDirection) > -seeer.Attributes.widthOfVision && !BehindObject(seeer, a.transform.position))
            {
                if (a.Shadow && !CheckShadow(seeer, name))
                {
                    return false;
                }
                return true;
            }
            return false;
        }
        catch (NullReferenceException e)
        {

            return false;
        }
    }

    // Defines if place is seen or not
    public bool CanSee(Agent seeer, Vector3 p)
    {
        try
        {
            Agent a = LevelManager.Instance.Agents.Find(x => x.name == name);
            Vector3 objectPosition = p;
            Vector3 seeerPosition = seeer.transform.position;


            float distanceToObject = (objectPosition - seeerPosition).magnitude;
            Vector3 directionToObject = (objectPosition - seeerPosition) / distanceToObject;

            if (distanceToObject < seeer.Attributes.reachOfVision && Vector3.Angle(directionToObject, seeer.visionDirection) < seeer.Attributes.widthOfVision
                && Vector3.Angle(directionToObject, seeer.visionDirection) > -seeer.Attributes.widthOfVision && !BehindObject(seeer, a.transform.position))
            {
                return true;
            }
            return false;
        }
        catch (NullReferenceException e)
        {

            return false;
        }
    }

    // Checks if the position is behind an object
    public bool BehindObject(Agent seeer, Vector3 a)
    {
        return Physics2D.Linecast(seeer.transform.position, a, LayerMask.GetMask("Walls"));
    }

    public Dictionary<Agent, Dictionary<Agent, int>> ShadowMap = new Dictionary<Agent, Dictionary<Agent, int>>();
    private int SpottingChance = 70;

    // Checks if the agent can see through the shadow
    private bool CheckShadow(Agent checker, string name)
    {
        Agent checkedAgent = Agents.Find(x => x.name == name);
        System.Random rand = new System.Random();
        
        if(checkedAgent != null && ShadowMap.ContainsKey(checkedAgent) )
        {
            Dictionary<Agent, int> seenList = ShadowMap[checkedAgent];
            
            if (!seenList.ContainsKey(checker))
            {
                seenList.Add(checker, rand.Next(100));
            }

            bool spotted = seenList[checker] > SpottingChance;
            Debug.Log(spotted + " " + seenList[checker]);

            if (spotted) {
                ShadowMap.Remove(checkedAgent);
                checkedAgent.Shadow = false;
                Debug.Log("HERE LEVEL MANAGER");
            }
            return spotted;
        }

        return false;
    }


    private void OnDrawGizmosSelected()
    {
        foreach (Vector2 v in StartTilesTeam1)
        {

            Gizmos.color = Color.red;

            Gizmos.DrawCube(v, Vector3.one * (0.3f - .1f));
        }

        foreach (Vector2 v in StartTilesTeam2)
        {

            Gizmos.color = Color.red;

            Gizmos.DrawCube(v, Vector3.one * (0.3f - .1f));
        }
    }


}
