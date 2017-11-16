using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class LevelManager : Singleton<LevelManager> {
    [SerializeField]
    private GameObject[] tilePrefabs;

    [SerializeField]
    private GameObject agent;

    public List<GameObject> Tiles { get; private set; }
    public List<GameObject> StartTilesTeam1 { get; private set; }
    public List<GameObject> StartTilesTeam2 { get; private set; }
    public List<GameObject> WallTiles { get; private set; }

    public List<Agent> Agents { get; private set; }
    public List<GameObject> Bullets { get; private set; } 


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


    public float TileSize
    {
        get
        {
            return tilePrefabs[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        }
    }

  


    // Use this for initialization
    void Start () {
        initialize();
        Vector3 worldStart = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height));
        CreateMap(worldStart);
        CreateAgents(worldStart);

    }

    // Update is called once per frame
    void Update () {
	
	}

    // For initialization
    private void initialize() {
        readMap();
        Tiles = new List<GameObject>();
        StartTilesTeam1 = new List<GameObject>();
        StartTilesTeam2 = new List<GameObject>();

        WallTiles = new List<GameObject>();
        Agents = new List<Agent>();
    }

    // Placeholder for reading file from map
    private void readMap()
    {
        string path = "Assets/Maps/TempMap.txt";
        string[] lines = System.IO.File.ReadAllLines(path);
        int ymax = lines.Length;
        int xmax = lines[0].Split().Length;

        map = new int[xmax, ymax];

        for(int y = 0; y<ymax; y++)
        {
            string[] values = lines[y].Split();

            for(int x = 0; x<xmax; x++)
            {                
                map[x, y] = Int32.Parse(values[x]);
            }

        }
    }

    // Placeholder for mapcreator
    private void CreateMap(Vector3 worldStart)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                PlaceTile(x, y, worldStart, map[x, y]);
            }
        }

        
    }

    //Placeholder for tilecreation
    private void PlaceTile(int x, int y, Vector3 worldStart, int type)
    {
        GameObject newTile = Instantiate(tilePrefabs[type]);
        Vector3 tilePosition = new Vector3(worldStart.x + TileSize / 2 + (TileSize * x), worldStart.y - TileSize / 2 - (TileSize * y));
        newTile.transform.position = tilePosition;

        // differentiate playing field-stadium
        if (type == 0 || type == 2)
        {
            Tiles.Add(newTile);
        }

        if (type == 2)
        {
            if (y > MaxYTiles / 2)
                StartTilesTeam1.Add(newTile);
            else
                StartTilesTeam2.Add(newTile);
        }

        if (type == 3)
        {
            WallTiles.Add(newTile);
        }
    }

    // Placeholder for agent spawning
    private void CreateAgents(Vector3 worldStart)
    {
        for (int i = 0; i< StartTilesTeam1.Count ; i++) //
        {

            Agent newAgent = ObjectManager.spawnAgent(new AgentAttributes(agent));
            newAgent.name = "Agent_" + i + "_Team1";
            newAgent.transform.position = StartTilesTeam1[i].transform.position;
            newAgent.Team = 1;
            Agents.Add(newAgent);
        }

        for (int i = 0; i < StartTilesTeam2.Count ; i++) //
        {

            Agent newAgent = ObjectManager.spawnAgent(new AgentAttributes(agent));
            newAgent.name = "Agent_" + i + "_Team2";
            newAgent.transform.position = StartTilesTeam2[i].transform.position;
            newAgent.Team = 2;
            newAgent.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
            Agents.Add(newAgent);
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

  

}
