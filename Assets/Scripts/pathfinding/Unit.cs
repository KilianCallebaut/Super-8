using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {
	
	public Vector2 Destination;

	Vector2[] path;
	int targetIndex;
    BoxCollider2D rightCollider;

	void Start() {

        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        for(int i = 0; i< colliders.Length; i++)
        {
            if (!colliders[i].isTrigger)
                rightCollider = colliders[i];
        }

        StartCoroutine(RefreshPath ());
	}


    IEnumerator RefreshPath() {
		Vector2 targetPositionOld = Destination + Vector2.up; // ensure != to target.position initially
			
		while (true) {
			if (targetPositionOld != Destination) {
				targetPositionOld = Destination;
                var pos = transform.position + new Vector3(transform.localScale.x * rightCollider.offset.x, transform.localScale.y * rightCollider.offset.y);

                path = Pathfinding.RequestPath (pos, Destination);
				StopCoroutine ("FollowPath");
				StartCoroutine ("FollowPath");
			}

			yield return new WaitForSeconds (.25f);
		}
	}
		
	IEnumerator FollowPath() {
		if (path.Length > 0) {
			targetIndex = 0;
			Vector2 currentWaypoint = path [0];

			while (true) {
                var speed = gameObject.GetComponent<Agent>().Attributes.speed;
                var pos = transform.position + new Vector3(transform.localScale.x * rightCollider.offset.x, transform.localScale.y * rightCollider.offset.y);

                if ((Vector2)pos == currentWaypoint) {
					targetIndex++;
					if (targetIndex >= path.Length) {
						yield break;
					}
					currentWaypoint = path [targetIndex];
				}
				transform.position = Vector2.MoveTowards (transform.position, transform.position - (pos - (Vector3) currentWaypoint) , speed * Time.deltaTime);
                GetComponent<Agent>().direction = transform.position - (pos - (Vector3)currentWaypoint);

                yield return null;

			}
		}
	}

	public void OnDrawGizmos() {
		if (path != null) {
			for (int i = targetIndex; i < path.Length; i ++) {
				Gizmos.color = Color.black;
                //Gizmos.DrawCube((Vector3)path[i], Vector3.one *.5f);
                var pos = transform.position + new Vector3(transform.localScale.x * rightCollider.offset.x, transform.localScale.y * rightCollider.offset.y);


                if (i == targetIndex) {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(pos , path[i]);

                }
                else {
                    Gizmos.color = Color.cyan;
					Gizmos.DrawLine(path[i-1] , path[i] );
				}
			}
		}
	}
}
