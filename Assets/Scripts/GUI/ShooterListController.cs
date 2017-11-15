using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterListController : MonoBehaviour {
    public GameObject ShooterList;
    public GameObject ShooterPrefab;
	
	void Start () {
        string[] names = new string[] { "bob", "alice", "henning", "alfons" };
        string[] status = new string[] { "YES", "YES", "NO", "YES" };
		for (int i = 0; i < 4; i++)
        {
            GameObject newShooter = GameObject.Instantiate(ShooterPrefab) as GameObject;
            ShooterController controller = newShooter.GetComponent<ShooterController>();
            controller.Name.text = names[i];
            controller.Status.text = status[i];
            newShooter.transform.parent = ShooterList.transform;
            newShooter.transform.localScale = Vector3.one;
        }
	}

}
