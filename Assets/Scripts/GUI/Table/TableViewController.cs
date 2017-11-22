using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tacticsoft;

//An example implementation of a class that communicates with a TableView
public class TableViewController : MonoBehaviour, ITableViewDataSource
{
    public TableCell CellPrefab;
    public TableView TableViewRef;

    private int m_numInstancesCreated = 0;
    private List<string> players;

    //Register as the TableView's delegate (required) and data source (optional)
    //to receive the calls
    void Start()
    {
        players = new List<string> { "Alice", "Bob", "Henning", "Arne", "Bettan" };
        TableViewRef.dataSource = this;
    }

    #region ITableViewDataSource

    //Will be called by the TableView to know how many rows are in this table
    public int GetNumberOfRowsForTableView(TableView tableView)
    {
        return players.Count;
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
            cell.name = "VisibleCounterCellInstance_" + (++m_numInstancesCreated).ToString();
        }
        cell.SetRowName(players[row]);
        return cell;
    }

    #endregion



}
