using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager> {
    [SerializeField]
    private GameObject[] tilePrefabs;

    [SerializeField]
    private GameObject agent;

    private int xsize = 14;
    private int ysize = 20;

    public GameObject[] Tiles { get; set; }

    public float MaxXTiles
    {
        get
        {
            return xsize;
        }
    }

    public float MaxYTiles
    {
        get
        {
            return ysize;
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
        Tiles = new GameObject[xsize * ysize];
        Vector3 worldStart = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height));
        CreateMap(worldStart);
        CreateAgents(worldStart);

    }

    // Update is called once per frame
    void Update () {
		
	}

    // Placeholder for mapcreator
    private void CreateMap(Vector3 worldStart)
    {
        for (int x = 0; x < xsize; x++)
        {
            for (int y = 0; y < ysize; y++)
            {
                PlaceTile(x, y, worldStart, 0);
            }
        }

        
    }

    //Placeholder for tilecreation
    private void PlaceTile(int x, int y, Vector3 worldStart, int type)
    {
        GameObject newTile = Instantiate(tilePrefabs[type]);
        Vector3 tilePosition = new Vector3(worldStart.x + TileSize/2 + (TileSize * x), worldStart.y - TileSize/2 - (TileSize * y));
        newTile.transform.position = tilePosition;
        Tiles[x * ysize + y] = newTile;
    }

    // Placeholder for agent spawning
    private void CreateAgents(Vector3 worldStart)
    {
        GameObject newAgent = Instantiate(agent);
        newAgent.transform.position = Tiles[0].transform.position;

    }


}
