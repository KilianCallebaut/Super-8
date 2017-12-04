using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBoxController : MonoBehaviour {

    private AgentAttributes attributes;
	private AgentStatus agent;
    private PlayersScreenController playersScreen;

    public Text Name;
    public Text SpeedValue;
    public Text ReachOfVisionValue;
    public Text WidthOfVisionValue;
    public Text ReflexValue;
    public Text AgilityValue;
    public Text AccuracyValue;
    public Button RestPlayButton;
    public Button RetireButton;
    public Dropdown TrainingDropdown;

	public void Initialize (PlayersScreenController playersScreen, AgentAttributes attributes, AgentStatus agent) {
        this.playersScreen = playersScreen;
        this.attributes = attributes;
		this.agent = agent;

		Name.text = agent.Name;
		SpeedValue.text = attributes.speed.ToString("0.0");
		ReachOfVisionValue.text = attributes.reachOfVision.ToString("0.0");
		WidthOfVisionValue.text = attributes.widthOfVision.ToString("0.0");
		ReflexValue.text = attributes.reflex.ToString("0.0");
		AgilityValue.text = attributes.agility.ToString("0.0");
		AccuracyValue.text = attributes.accuracy.ToString("0.0");

        TrainingDropdown.value = (int) agent.Training;
        TrainingDropdown.onValueChanged.AddListener(TrainingChanged);

        UpdateRestPlayLabel();
        RestPlayButton.onClick.AddListener(RestPlayClick);
        RetireButton.onClick.AddListener(RetireClick);
	}

    void TrainingChanged(int newValue)
    {
        agent.Training = (AgentStatus.TrainingEnum) newValue;
    }

    public void UpdateRestPlayLabel()
    {
        Text restPlayButtonLabel = RestPlayButton.transform.Find("Label").GetComponent<Text>();
        restPlayButtonLabel.text = (agent.Active ? "Rest" : "Activate");
    }

    void RestPlayClick()
    {
        if (agent.Active) playersScreen.DeactivateAgent(this, agent);
        else playersScreen.ActivateAgent(this, agent);
        UpdateRestPlayLabel();
    }

    void RetireClick()
    {
        playersScreen.RemovePlayerBox(agent, true);
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
    }

    public void Destroy()
    {
        TrainingDropdown.onValueChanged.RemoveAllListeners();
        RestPlayButton.onClick.RemoveAllListeners();
        RetireButton.onClick.RemoveAllListeners();
        Destroy(gameObject);
    }



}
