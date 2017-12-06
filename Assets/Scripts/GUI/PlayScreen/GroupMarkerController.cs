using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupMarkerController : MonoBehaviour {

    private PlayScreenController playScreen;

	public void Initialize(PlayScreenController playScreen, int groupIndex)
    {
        this.playScreen = playScreen;
        Image img = GetComponent<Image>();
        img.color = PlayScreenController.GROUP_COLORS[groupIndex];
    }

    public void UpdateVisibility(int groupSize)
    {
        if (groupSize == 0 && gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        else if (groupSize > 0 && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }
}
