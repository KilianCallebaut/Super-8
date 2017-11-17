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

	[SerializeField]
	private Texture2D mapTexture;

    public List<GameObject> Tiles { get; private set; }
    public List<GameObject> StartTilesTeam1 { get; private set; }
    public List<GameObject> StartTilesTeam2 { get; private set; }
    public List<GameObject> WallTiles { get; private set; }

    public List<Agent> Agents { get; private set; }
    public List<GameObject> Bullets { get; private set; } 

	private GameObject tile_dump;
	private GameObject team1_parent;
	private GameObject team2_parent;

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
        //CreateMap(worldStart);
        CreateAgents(worldStart);

    }

    // Update is called once per frame
    void Update () {
	
	}

	private void readMapTexture () {
		Color[] pixels = mapTexture.GetPixels (0, 0, mapTexture.width, mapTexture.height);
		for (int y = 0; y < mapTexture.height; y++) {
			for (int x = 0; x < mapTexture.width; x++) {
				Color c = pixels [(y * mapTexture.width) + x];
				GameObject tile = new GameObject ();
				tile.transform.position = new Vector3 (x, y);
				tile.transform.SetParent (tile_dump.transform);
				if (c == Color.black) {
					WallTiles.Add (tile);
					tile.name = "Wall";
				} else if (c == Color.blue) {
					StartTilesTeam1.Add (tile);
					tile.name = "Team 1";
				} else if (c == Color.green) {
					StartTilesTeam2.Add (tile);
					tile.name = "Team 2";
				} else {
					Tiles.Add (tile);
					tile.name = "Walkable";
				}
			}
		}
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
        StartTilesTeam1 = new List<GameObject>();
        StartTilesTeam2 = new List<GameObject>();

        WallTiles = new List<GameObject>();
        Agents = new List<Agent>();

		readMapTexture();
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
        for (int i = 0; i< StartTilesTeam1.Count; i++) //
        {

            Agent newAgent = ObjectManager.spawnAgent(new AgentAttributes(agent));
            newAgent.name = "Agent_" + i + "_Team1";
            newAgent.transform.position = StartTilesTeam1[i].transform.position;
            newAgent.Team = 1;
            Agents.Add(newAgent);
			newAgent.transform.SetParent (team1_parent.transform);
        }

        for (int i = 0; i < StartTilesTeam2.Count; i++) //
        {

            Agent newAgent = ObjectManager.spawnAgent(new AgentAttributes(agent));
            newAgent.name = "Agent_" + i + "_Team2";
            newAgent.transform.position = StartTilesTeam2[i].transform.position;
            newAgent.Team = 2;
            Agents.Add(newAgent);
			newAgent.transform.SetParent (team2_parent.transform);
        }
    }

    // Placeholder for agent death
    public void DeleteAgent(Agent deletedAgent)
    {
        Destroy(deletedAgent.gameObject);
        Agents.Remove(deletedAgent);

    }


}
