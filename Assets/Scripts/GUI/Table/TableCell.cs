using UnityEngine;
using System.Collections;
using Tacticsoft;
using UnityEngine.UI;

//Inherit from TableViewCell instead of MonoBehavior to use the GameObject
//containing this component as a cell in a TableView
public class TableCell : TableViewCell
{
    public Text NameText;
	public Text SpeedValue;
	public Text ReachOfVisionValue;
	public Text WidthOfVisionValue;
	public Text ReflexValue;
	public Text AgilityValue;
	public Text AccuracyValue;
	public Text CostValue;
	public Button BuyButton;

	private MarketTableController marketTable;
	private AgentStatus agent;

	public void Initialize(MarketTableController marketTable) {
		this.marketTable = marketTable;
	}

    public void SetRowAgent(AgentStatus agent)
    {
		this.agent = agent;

		NameText.text = agent.Name;
		SpeedValue.text = agent.Attributes.speed.ToString("0.0");
		ReachOfVisionValue.text = agent.Attributes.reachOfVision.ToString("0.0");
		WidthOfVisionValue.text = agent.Attributes.widthOfVision.ToString("0.0");
		ReflexValue.text = agent.Attributes.reflex.ToString("0.0");
		AgilityValue.text = agent.Attributes.agility.ToString("0.0");
		AccuracyValue.text = agent.Attributes.accuracy.ToString("0.0");
		CostValue.text = agent.Cost.ToString("0.0") + "$";

		BuyButton.onClick.AddListener (BuyAgent);
    }

	public void ClearEvents() {
		BuyButton.onClick.RemoveAllListeners ();
	}

	void BuyAgent() {
		marketTable.BuyFreeAgent (agent);
	}

}
