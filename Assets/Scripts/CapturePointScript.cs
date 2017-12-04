using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturePointScript : MonoBehaviour {
	LinkedList<GameObject> colliders = new LinkedList<GameObject>();
	LinkedList<GameObject> deleteList = new LinkedList<GameObject>();

	// Use this for initialization
	void Start () {
		
	}


	void OnTriggerEnter2D(Collider2D c) {
		if(c.gameObject.GetComponent<Agent>() != null)
			colliders.AddLast (c.gameObject);
	}

	void OnTriggerExit2D(Collider2D c) {
		LinkedListNode<GameObject> d = colliders.Find (c.gameObject);
		if(d!=null && d.Value.GetComponent<Agent>() != null)
			colliders.Remove (d);
	}
	//returns the capture points for owning this point for both teams
	public Vector2 getCapturepointIncrease() {
		Vector2 pointProgress = new Vector2();
		if (captureTugOfWar == 100.0f) {
			pointProgress.x = 1;
			pointProgress.y = 0;
		} else if (captureTugOfWar == -100.0f) {
			pointProgress.x = 0;
			pointProgress.y = 1;
		} else {
			pointProgress.x = 0;
			pointProgress.y = 0;
		}
		return pointProgress * UnityEngine.Time.deltaTime * 10;
	}
	// Update is called once per frame
	float captureTugOfWar = 0;

	void Update () {
		//iterate through the colliding colliders and find which 
		int team1 = 0;
		int team2 = 0;

		foreach (GameObject g in colliders) {
			if (g == null) {
				deleteList.AddLast (g);
				continue;
			}

			if (g.GetComponent<Agent> ().Team == 1) {
				team1++;
			} else {
				team2++;
			}
		}

		if (team1 > team2) {//team1 is capturing at constant rate since they outnumber team2 at capture point
			captureTugOfWar += 10.0f * UnityEngine.Time.deltaTime;
			if (captureTugOfWar > 100.0f)
				captureTugOfWar = 100.0f;
		} else if (team2 > team1) {//team2 is capturing at constant rate since they outnumber team1 at capture point
			captureTugOfWar -= 10.0f * UnityEngine.Time.deltaTime;
			if (captureTugOfWar < -100.0f)
				captureTugOfWar = -100.0f;
		}

		foreach (GameObject g in deleteList) {
			colliders.Remove (colliders.Find (g));
		}

		deleteList.Clear();
	}
}
