using UnityEngine;
using System.Collections;
using Tacticsoft;
using UnityEngine.UI;

//Inherit from TableViewCell instead of MonoBehavior to use the GameObject
//containing this component as a cell in a TableView
public class TableCell : TableViewCell
{
    public Text NameText;

    public void SetRowName(string rowName)
    {
        NameText.text = rowName;
    }

}
