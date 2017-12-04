using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectivePlacement : MonoBehaviour {

	public GameObject ObjectiveMarkerPrefab;
	public GameObject LevelManager;

	private GameObject objectiveMarker;
	private bool markerIsActive = false;
	private bool markerIsPlaced = false;

	void Start () {
		objectiveMarker = GameObject.Instantiate(ObjectiveMarkerPrefab) as GameObject;
		objectiveMarker.transform.parent = transform;
	}

	private void UpdateMarker() {
		Vector2 to = Input.mousePosition;

		objectiveMarker.transform.position = to;
	}

	public void SendObjectiveToLevelManager()
	{
		if (!markerIsPlaced)
			return;
		
		LevelManager actualLevelManager = LevelManager.GetComponent<LevelManager> ();
		Camera c = Camera.main;
		Vector3 objective = c.ScreenToWorldPoint (objectiveMarker.transform.position);
		objective.z = 0;
		actualLevelManager.AddObjectiveForTeam1 (objective);
        Debug.DrawLine(objective, Vector3.zero);
	}

	public void ActivateMarker()
	{
		markerIsActive = true;
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0)) // left click
		{
			if (markerIsActive) {
				markerIsActive = false;
				markerIsPlaced = true;
			}
		}

		if (markerIsActive)
			UpdateMarker();
	}
}