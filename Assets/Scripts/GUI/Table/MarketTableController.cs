using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tacticsoft;

//An example implementation of a class that communicates with a TableView
public class MarketTableController : MonoBehaviour, ITableViewDataSource
{
    public TableCell CellPrefab;
    public TableView TableViewRef;
	public TeamManager TeamManagerRef;
	public MarketManager MarketManagerRef;

    private int m_numInstancesCreated = 0;
	private List<AgentStatus> freeAgents;

	//Register as the TableView's delegate (required) and data source (optional)
	//to receive the calls
	void Start() {
		// THIS SHIT WAS MOVED TO ADD_FREE_AGENTS, MAYBE THAT'S A BAD IDEA?

		//freeAgents = new List<AgentStatus>();
		//TableViewRef.dataSource = this;
	}

	public void AddFreeAgents(List<AgentStatus> agents) {
		freeAgents = new List<AgentStatus>();
		freeAgents.AddRange(agents.Where(agent => agent.FreeAgent));
		TableViewRef.dataSource = this;
	}

	public void BuyFreeAgent(AgentStatus agent) {
		if (!TeamManagerRef.CanBuyAgent(agent)) return;

		freeAgents.Remove (agent);
		TableViewRef.ReloadData ();
		MarketManagerRef.BuyAgent (agent);
	}

    #region ITableViewDataSource

    //Will be called by the TableView to know how many rows are in this table
    public int GetNumberOfRowsForTableView(TableView tableView)
    {
        return freeAgents.Count;
    }

    //Will be called by the TableView to know what is the height of each row
    public float GetHeightForRowInTableView(TableView tableView, int row)
    {
        return (CellPrefab.transform as RectTransform).rect.height;
    }

    //Will be called by the TableView when a cell needs to be created for display
    public TableViewCell GetCellForRowInTableView(TableView tableView, int row)
    {
        TableCell cell = tableView.GetReusableCell(CellPrefab.reuseIdentifier) as TableCell;
        if (cell == null)
        {
            cell = (TableCell)GameObject.Instantiate(CellPrefab);
			cell.Initialize (this);
            cell.name = "VisibleCounterCellInstance_" + (++m_numInstancesCreated).ToString();
        }
		cell.ClearEvents ();
        cell.SetRowAgent(freeAgents[row]);
        return cell;
    }

    #endregion



}
