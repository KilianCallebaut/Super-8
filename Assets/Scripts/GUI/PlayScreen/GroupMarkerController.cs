using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GroupMarkerController : MonoBehaviour {

    private PlayScreenController playScreen;
    private Vector3 initialPosition;

    private bool markerIsActive = false;
    private bool markerIsPlaced = false;

    public void Initialize(PlayScreenController playScreen, int groupIndex, Transform parent, Vector3 initialPosition)
    {
        this.playScreen = playScreen;
        this.initialPosition = initialPosition;
        transform.SetParent(parent);
        transform.localPosition = initialPosition;
        Image img = GetComponent<Image>();
        img.color = PlayScreenController.GROUP_COLORS[groupIndex];
    }

    public void SendObjectiveToLevelManager()
    {
        if (!markerIsPlaced)
            return;

        //LevelManager actualLevelManager = LevelManager.GetComponent<LevelManager>();
        Camera c = Camera.main;
        Vector3 objective = c.ScreenToWorldPoint(transform.position);
        objective.z = 0;
        //actualLevelManager.AddObjectiveForTeam1(objective);
        Debug.DrawLine(objective, Vector3.zero);
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

    private void UpdateMarker()
    {
        Vector2 to = Input.mousePosition;

        transform.position = to;
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
            GraphicRaycaster gr = playScreen.CanvasRef.GetComponent<GraphicRaycaster>();
            PointerEventData ped = new PointerEventData(null);
            ped.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            gr.Raycast(ped, results);

            // if (this) node is in the raycast results, do something
            if (results.Find((result) => result.gameObject == gameObject).gameObject != null)
            {
                if (markerIsActive)
                {
                    // TODO: if out of bounds, reset to initial position
                    markerIsActive = false;
                    markerIsPlaced = true;
                } else
                {
                    markerIsActive = true;
                }
            }
        }

        if (Input.GetMouseButtonDown(1)) // right click
        {
            if (markerIsActive)
            {
                markerIsActive = false;
                transform.localPosition = initialPosition;
            }
        }

        if (markerIsActive)
            UpdateMarker();
    }
}
