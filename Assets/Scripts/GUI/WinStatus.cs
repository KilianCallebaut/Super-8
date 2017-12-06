using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinStatus : MonoBehaviour {

	public Text WinText;
	public LevelManager LevelManagerRef;
    public Canvas MenuCanvasRef;

	private bool winConditionMet = false;

	private CapturePointScript[] capturePoints;
	public Text captureText;

	void Start()
	{
		capturePoints = FindObjectsOfType (typeof(CapturePointScript)) as CapturePointScript[];
		if (captureText != null)
			captureText.gameObject.SetActive (true);
	}

    public void Reset()
    {
        winConditionMet = false;
    }

	IEnumerator CheckWinCondition()
	{
		int aliveTeam1 = 0;
		int aliveTeam2 = 0;
		foreach (var agent in LevelManagerRef.Agents) {
			if (agent.Team == 1)
				aliveTeam1 += 1;
			else if (agent.Team == 2)
				aliveTeam2 += 1;
		}

		if (aliveTeam1 == 0 || captureProgress.y >= 100.0f) {
			WinText.text = "YOU LOSE";
			WinText.gameObject.SetActive (true);
			winConditionMet = true;
            Invoke("BackToMenu", 1);
        } else if (aliveTeam2 == 0|| captureProgress.x >= 100.0f) {
			WinText.text = "YOU WIN";
			WinText.gameObject.SetActive (true);
			winConditionMet = true;
            Invoke("BackToMenu", 1);
        }


        yield return new WaitForSeconds(1);
	}

    void BackToMenu()
    {
        LevelManagerRef.DestroyEverything();
        MenuCanvasRef.gameObject.SetActive(true);
        WinText.text = "";
        WinText.gameObject.SetActive(false);
        // do something with the level?
    }

    void FixedUpdate()
	{
		if (!winConditionMet && LevelManagerRef.Agents.Count > 0)
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
