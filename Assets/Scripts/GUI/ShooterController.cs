using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShooterController : MonoBehaviour {
	public static readonly List<string> Roles = new List<string>() { "Tank", "Cop", "Mailman", "Pilot" };

    public Text Name;
	public GameObject RoleDropdown;

	private Dropdown dropdown;

	void Awake () {
		dropdown = RoleDropdown.GetComponent<Dropdown> ();
		dropdown.ClearOptions ();
		dropdown.AddOptions (Roles);
	}

	public void SetDropdownValue (string role) {
		int dropdownIndex = Roles.IndexOf (role);

		if (dropdownIndex != -1) {
			dropdown.value = dropdownIndex;
		} else {
			Debug.LogError ("Non-existant role assigned to shooter dropdown value");
		}
	}
}
