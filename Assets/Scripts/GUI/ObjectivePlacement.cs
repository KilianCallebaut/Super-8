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
		LevelManager actualLevelManager = LevelManager.GetComponent<LevelManager> ();
		Camera c = Camera.main;
		Vector3 objective = c.ScreenToWorldPoint (objectiveMarker.transform.position);
		objective.z = 0;
		actualLevelManager.AddObjectiveForTeam1 (objective);
	}

	public void ActivateMarker()
	{
		markerIsActive = true;
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0)) // left click
		{
			// do a graphics raycast on the canvas
			GraphicRaycaster gr = GetComponent<GraphicRaycaster> ();
			PointerEventData ped = new PointerEventData(null);
			ped.position = Input.mousePosition;
			List<RaycastResult> results = new List<RaycastResult>();
			gr.Raycast(ped, results);

			if (markerIsActive)
				markerIsActive = false;

			// if (this) node is in the raycast results, do something
			if (results.Find((result) => result.gameObject == gameObject).gameObject != null)
			{
				
			}
		}

		/*if (Input.GetMouseButtonDown(1)) // right click
		{
			if (arrowIsActive)
			{
				// destroy the arrow
				arrow.SetActive(false);
				arrowIsActive = false;
			}
		}*/

		// update the arrow so that it ends at the mouse pointer
		if (markerIsActive)
			UpdateMarker();
	}
}