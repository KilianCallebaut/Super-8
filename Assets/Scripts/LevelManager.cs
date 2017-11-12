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
    public List<GameObject> StartTiles { get; private set; }
    public List<GameObject> Agents { get; private set; }
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
        StartTiles = new List<GameObject>();
        Agents = new List<GameObject>();
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
        if (type == 0 || type == 3)
        {
            Tiles.Add(newTile);
        }

        if (type == 2)
        {
            StartTiles.Add(newTile);
        }
    }

    // Placeholder for agent spawning
    private void CreateAgents(Vector3 worldStart)
    {
        for (int i = 0; i< 4; i++) //StartTiles.Count
        {
            GameObject newAgent = Instantiate(agent);
            newAgent.name = "Agent_" + i;
            newAgent.transform.position = StartTiles[i].transform.position;
            Agents.Add(newAgent);
        }
    }

    // Placeholder for agent death
    public void DeleteAgent(GameObject deletedAgent)
    {
        Destroy(deletedAgent);
        Agents.Remove(deletedAgent);

    }


}
