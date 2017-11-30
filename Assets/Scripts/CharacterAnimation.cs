using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour {

	// Aiming Arm
	public GameObject gunArm;
	public float aimSpeed;

	private Agent thisAgent;
	private GameObject head;
	private float armAngle;
	private int direction;
	private float maxHeadTilt = 30; // degrees

	void Start(){
		thisAgent = null;
		if (GetComponent<Agent> () != null) {
			thisAgent = GetComponent<Agent> ();
			direction = thisAgent.visionDirection.x > 0 ? -1 : 1;
			Debug.Log ("Agent script attached");
		} else {
			Debug.Log ("Agent script missing");
		}
		head = transform.Find("head").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if (thisAgent != null) {

			// Check turn direction
			if (thisAgent.visionDirection.x < 0 && direction < 0 ||
				thisAgent.visionDirection.x > 0 && direction > 0) {

				direction = -1 * direction;
				transform.localScale = new Vector3 (direction, 1, 1);
			}

			// Check target
			if (thisAgent.TargetAgent != null) {
				aimTowards (gunArm, thisAgent.TargetAgent.LastPosition, 90);
				headTilt (thisAgent.TargetAgent.LastPosition);
			} else {
				aimTowards (gunArm, gunArm.transform.position + Vector3.down, 90);
				headTilt (head.transform.position + (Vector3.left * direction));
			}
		}
	}

	void aimTowards(GameObject aimer, Vector3 target, float rotationOffset = 0){
		Vector3 aimVector = (target - gunArm.transform.position).normalized;

		float targetAngle = Mathf.Atan2(aimVector.y, aimVector.x) * Mathf.Rad2Deg;
		gunArm.transform.rotation = Quaternion.Euler (0f, 0f, targetAngle + 90);
	}

	void headTilt(Vector3 target){
		Vector3 aimVector = (target - head.transform.position).normalized;

		float targetAngle = Mathf.Atan2(aimVector.y, aimVector.x) * Mathf.Rad2Deg;

		if (direction > 0)
			targetAngle += 180;

		Quaternion q = Quaternion.AngleAxis(targetAngle, Vector3.forward);
		head.transform.rotation = Quaternion.Slerp(head.transform.rotation, q, Time.deltaTime * aimSpeed);
	}
}
