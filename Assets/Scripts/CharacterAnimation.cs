using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour {

	// Aiming Arm
	public GameObject gunArm;
	public float aimSpeed;

	private Agent thisAgent;
	private Animator animator;
	private GameObject head;
	private float armAngle;
	private int direction;
	private float maxHeadTilt = 30; // degrees

	void Start(){
		thisAgent = null;
		animator = GetComponent<Animator> ();

		if (GetComponent<Agent> () != null) {
			thisAgent = GetComponent<Agent> ();
			direction = thisAgent.visionDirection.x > 0 ? -1 : 1;
		} else {
			Debug.Log ("Agent script missing");
		}

		head = transform.Find("head").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if (thisAgent != null) {

			updateAnimationState ();

			// Check turn direction
			if (thisAgent.visionDirection.x < 0 && direction < 0 ||
				thisAgent.visionDirection.x > 0 && direction > 0) {

				direction = -1 * direction;
				transform.localScale = new Vector3 (direction, 1, 1);
			}

			// Check target
			if (thisAgent.TargetAgent != null) {
				animator.SetBool ("gotTarget", true);
				aimTowards (gunArm, thisAgent.TargetAgent.LastPosition, 90);
				headTilt (thisAgent.TargetAgent.LastPosition);
			} else {
				animator.SetBool ("gotTarget", false);
			}
		}
	}

	void aimTowards(GameObject aimer, Vector3 target, float rotationOffset = 0){
		Vector3 aimVector = (target - gunArm.transform.position).normalized;
		Debug.DrawLine (gunArm.transform.position, target);

		float targetAngle = Vector3.Angle (Vector3.down, aimVector);
		
		Debug.Log (targetAngle);
		animator.SetFloat ("Angle", targetAngle / 180);
		//gunArm.transform.rotation = Quaternion.Euler (0f, 0f, targetAngle + 90);
	}

	void headTilt(Vector3 target){
		Vector3 aimVector = (target - head.transform.position).normalized;
		Debug.DrawLine (gunArm.transform.position, target);

		float targetAngle = Mathf.Atan2(aimVector.y, aimVector.x) * Mathf.Rad2Deg;

		if (direction > 0)
			targetAngle += 180;

		Quaternion q = Quaternion.AngleAxis(targetAngle, Vector3.forward);
		head.transform.rotation = Quaternion.Slerp(head.transform.rotation, q, Time.deltaTime * aimSpeed);
	}

	void updateAnimationState(){
		if (thisAgent.Behaviour.positioning == AgentBehaviour.PositioningMethod.Stop) {
			animator.SetBool ("isMoving", false);
		} else {
			animator.SetBool ("isMoving", true);
		}
	}
}
