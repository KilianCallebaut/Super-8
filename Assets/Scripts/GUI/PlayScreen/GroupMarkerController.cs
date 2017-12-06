using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GroupMarkerController : MonoBehaviour {

    private PlayScreenController playScreen;
    private GameObject container;
    private Vector3 initialPosition;

    private bool markerIsActive = false;

    public void Initialize(PlayScreenController playScreen, int groupIndex, Transform parent, Vector3 initialPosition)
    {
        this.playScreen = playScreen;
        this.initialPosition = initialPosition;
        this.container = parent.gameObject;
        transform.SetParent(parent);
        transform.localPosition = initialPosition;
        Image img = GetComponent<Image>();
        img.color = PlayScreenController.GROUP_COLORS[groupIndex];
    }

    public Vector3 ToWorldSpace()
    {
        Vector3 screenSpace = transform.localPosition;
        Rect containerRect = container.GetComponent<RectTransform>().rect;
        screenSpace.x *= Screen.height / containerRect.height; // not sure about this
        screenSpace.y *= Screen.height / containerRect.height;

        Camera c = Camera.main;
        Vector3 objective = c.ScreenToWorldPoint(screenSpace);

        Scene scene = SceneManager.GetActiveScene();
        GameObject map = scene.GetRootGameObjects().FirstOrDefault(go => go.name == "Map");

        return objective + map.transform.position;
    }

    public void SendObjectiveToLevelManager()
    {
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

    public bool MarkerIsInBounds()
    {
        Vector3 pos = transform.localPosition;
        Rect bounds = container.GetComponent<RectTransform>().rect;
        return !(pos.x < 0 || pos.y < 0 || pos.x > bounds.width || pos.y > bounds.height);
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
                    markerIsActive = false;
                    if (!MarkerIsInBounds())
                        transform.localPosition = initialPosition;
                    else
                        markerIsActive = false;
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
