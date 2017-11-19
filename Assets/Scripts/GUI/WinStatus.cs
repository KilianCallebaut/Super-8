using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinStatus : MonoBehaviour {

	public Text WinText;
	public GameObject LevManager;

	private LevelManager levelManager;
	private bool winConditionMet = false;

	void Start()
	{
		levelManager = LevManager.GetComponent<LevelManager> ();
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

		if (aliveTeam1 == 0) {
			WinText.text = "YOU LOSE";
			WinText.gameObject.SetActive (true);
			winConditionMet = true;
		} else if (aliveTeam2 == 0) {
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

}
