using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour {

    public GameObject PlayersScreen;
    public GameObject PlayerBoxPrefab;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < 8; ++i)
        {
            var playerBox = Instantiate(PlayerBoxPrefab);
            playerBox.transform.parent = PlayersScreen.transform;
            PlayerBoxController controller = playerBox.gameObject.GetComponent<PlayerBoxController>();
            controller.Attributes = new AgentAttributes(null);
            //agents.Add(new AgentAttributes(null));
        }


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
