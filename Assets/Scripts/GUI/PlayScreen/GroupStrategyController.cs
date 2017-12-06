using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupStrategyController : MonoBehaviour {

    public Text GroupName;
    public Text NoOfMembers;
    public Text Objective;

    private PlayScreenController playScreen;

	public void Initialize(PlayScreenController playScreen)
    {
        this.playScreen = playScreen;
        gameObject.SetActive(false);
    }

    public void RefreshGroup(int index, int noOfMembers, string objective)
    {
        if (noOfMembers == 0 && gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            playScreen.ForceRefreshGroupHack();
            return;
        }

        GroupName.text = PlayScreenController.GROUP_NAMES[index];
        GroupName.color = PlayScreenController.GROUP_COLORS[index];
        NoOfMembers.text = noOfMembers.ToString();
        Objective.text = objective == "" ? Objective.text : objective;

        if (noOfMembers > 0 && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            playScreen.ForceRefreshGroupHack();
        }

    }
}
