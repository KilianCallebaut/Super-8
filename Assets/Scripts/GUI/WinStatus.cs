using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinStatus : MonoBehaviour {

	public Text WinText;
	public GameObject LevManager;


	private LevelManager levelManager;
	private bool winConditionMet = false;

	private CapturePointScript[] capturePoints;
	public Text captureText;

	void Start()
	{
		levelManager = LevManager.GetComponent<LevelManager> ();
		capturePoints = FindObjectsOfType (typeof(CapturePointScript)) as CapturePointScript[];
		if (captureText != null)
			captureText.gameObject.SetActive (true);
	}

	IEnumerator CheckWinCondition()
	{
		int aliveTeam1 = 0;
		int aliveTeam2 = 0;
		foreach (var agent in levelManager.Agents) {
			if (agent.Team == 1)
				aliveTeam1 += 1;
			else if (agent.Team == 2)
				aliveTeam2 += 1;
		}

		if (aliveTeam1 == 0 || captureProgress.y >= 100.0f) {
			WinText.text = "YOU LOSE";
			WinText.gameObject.SetActive (true);
			winConditionMet = true;
		} else if (aliveTeam2 == 0|| captureProgress.x >= 100.0f) {
			WinText.text = "YOU WIN";
			WinText.gameObject.SetActive (true);
			winConditionMet = true;
		}

		yield return new WaitForSeconds(1);
	}

	void FixedUpdate()
	{
		if (!winConditionMet && levelManager.Agents.Count > 0)
			StartCoroutine (CheckWinCondition());
	}

	Vector2 captureProgress = new Vector2(0,0);
	void Update() {
		//get the capture point progress
		foreach(CapturePointScript c in capturePoints) {
			captureProgress += c.getCapturepointIncrease ();
			captureProgress.x = captureProgress.x > 100 ? 100 : captureProgress.x;
			captureProgress.y = captureProgress.y > 100 ? 100 : captureProgress.y;
		}
		if(captureText != null) captureText.text = "" + captureProgress.x + "/100\n" + captureProgress.y + "/100";
	}

}
