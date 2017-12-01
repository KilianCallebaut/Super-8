using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBoxController : MonoBehaviour {

    private AgentAttributes attributes;
	private AgentStatus status;

    public Text Name;
    public Text SpeedValue;
    public Text ReachOfVisionValue;
    public Text WidthOfVisionValue;
    public Text ReflexValue;
    public Text AgilityValue;
    public Text AccuracyValue;

	void Initialize (AgentAttributes attributes, AgentStatus status) {
		this.attributes = attributes;
		this.status = status;

		Name.text = "Temp Name";
		SpeedValue.text = attributes.speed.ToString("0.0");
		ReachOfVisionValue.text = attributes.reachOfVision.ToString("0.0");
		WidthOfVisionValue.text = attributes.widthOfVision.ToString("0.0");
		ReflexValue.text = attributes.reflex.ToString("0.0");
		AgilityValue.text = attributes.agility.ToString("0.0");
		AccuracyValue.text = attributes.accuracy.ToString("0.0");
	}
}
