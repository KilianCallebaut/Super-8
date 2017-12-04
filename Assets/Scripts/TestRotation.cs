using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotation : MonoBehaviour {

	public GameObject target;
	public float maxTilt;

	private int direction = 1;

	void headTilt(Vector3 target){
		Vector3 aimVector = (target - transform.position).normalized;
		Debug.DrawLine (transform.position, target);

		float targetAngle = Mathf.Atan2(aimVector.y, aimVector.x) * Mathf.Rad2Deg;
		//float testAngle = Vector2.Angle (head.transform. + (Vector3.left * direction), new Vector2 (aimVector.x, aimVector.z));

		Debug.Log (targetAngle);
		if (direction > 0)
			targetAngle += 180;
		/*
		if (targetAngle > maxHeadTilt) {
			targetAngle = maxHeadTilt;
		} else if (targetAngle < maxHeadTilt * -1) {
			targetAngle = maxHeadTilt * -1;
		}
		Debug.Log (targetAngle);
		*/
		Quaternion q = Quaternion.AngleAxis(targetAngle, Vector3.forward);
		transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime);

	}
	
	// Update is called once per frame
	void Update () {
		if ((target.transform.position - transform.position).x > 0)
			direction = -1;
		else
			direction = 1;
		transform.localScale = new Vector3 (direction, 1, 1);
		headTilt (target.transform.position);
	}
}
