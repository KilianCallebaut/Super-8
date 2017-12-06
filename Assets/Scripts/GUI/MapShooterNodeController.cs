using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapShooterNodeController : MonoBehaviour {

	public GameObject Map;
	public GameObject ArrowPrefab;
    public Canvas CanvasRef;

    private GameObject arrow;
    private RectTransform rectTransform;
    private RectTransform arrowRectTransform;

    private bool arrowIsActive = false;

    void Start () {
        arrow = GameObject.Instantiate(ArrowPrefab) as GameObject;
        arrow.transform.parent = transform.parent;

        rectTransform = GetComponent<RectTransform>();
        arrowRectTransform = arrow.GetComponent<RectTransform>();
        arrowRectTransform.anchoredPosition = rectTransform.anchoredPosition;
        arrow.SetActive(false);
    }

    private void UpdateArrow() {
        Vector2 from = transform.position;
        Vector2 to = Input.mousePosition;

        // the height of the arrow
        float distance = Vector2.Distance(from, to);

        // the z-rotation of the arrow (from {0, 1} because that is the default orientation of the arrow)
        float angle = Vector2.SignedAngle(new Vector2(0, 1), to - from); 
        Vector3 rotation = arrowRectTransform.localEulerAngles;
        rotation.z = angle;

        // set the values so that the arrow starts at the node and ends at the mouse pointer
        arrowRectTransform.anchoredPosition = rectTransform.anchoredPosition;
        arrowRectTransform.sizeDelta = new Vector2(arrowRectTransform.sizeDelta.x, distance);
        arrowRectTransform.localEulerAngles = rotation;
	}

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // left click
        {
            // do a graphics raycast on the canvas
            GraphicRaycaster gr = CanvasRef.GetComponent<GraphicRaycaster> ();
            PointerEventData ped = new PointerEventData(null);
            ped.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            gr.Raycast(ped, results);

            // if (this) node is in the raycast results, do something
            if (results.Find((result) => result.gameObject == gameObject).gameObject != null)
            {
                if (!arrow.activeSelf)
                    arrow.SetActive(true);
                arrowIsActive = true;
            }
            else
            {
                arrowIsActive = false;
            }
        }

        if (Input.GetMouseButtonDown(1)) // right click
        {
            if (arrowIsActive)
            {
                // destroy the arrow
                arrow.SetActive(false);
                arrowIsActive = false;
            }
        }

        // update the arrow so that it ends at the mouse pointer
        if (arrowIsActive)
            UpdateArrow();
    }
}
