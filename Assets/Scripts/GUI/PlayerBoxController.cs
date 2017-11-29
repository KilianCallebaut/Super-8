using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBoxController : MonoBehaviour {

    private AgentAttributes _Attributes;
    public AgentAttributes Attributes {
        private get { return _Attributes; }
        set
        {
            _Attributes = value;
            Name.text = "Temp Name";
            SpeedValue.text = value.speed.ToString("0.0");
            ReachOfVisionValue.text = value.reachOfVision.ToString("0.0");
            WidthOfVisionValue.text = value.widthOfVision.ToString("0.0");
            ReflexValue.text = value.reflex.ToString("0.0");
            AgilityValue.text = value.agility.ToString("0.0");
            AccuracyValue.text = value.accuracy.ToString("0.0");
        }
    }

    public Text Name;
    public Text SpeedValue;
    public Text ReachOfVisionValue;
    public Text WidthOfVisionValue;
    public Text ReflexValue;
    public Text AgilityValue;
    public Text AccuracyValue;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
