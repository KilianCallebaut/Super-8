using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterListController : MonoBehaviour {
    public GameObject ShooterList;
    public GameObject ShooterPrefab;
	
	void Start () {
		Shooter[] shooters = new Shooter[] {
			new Shooter ("Alice", "Tank"),
			new Shooter ("Bob", "Cop"),
			new Shooter ("Henning", "Mailman"),
			new Shooter ("Alfons", "Pilot")
		};
		for (int i = 0; i < 4; i++)
        {
            GameObject newShooter = GameObject.Instantiate(ShooterPrefab) as GameObject;
			ShooterController controller = newShooter.GetComponent<ShooterController>();
			controller.Initialize (shooters[i]);
            newShooter.transform.parent = ShooterList.transform;
            newShooter.transform.localScale = Vector3.one;
        }
	}



}
