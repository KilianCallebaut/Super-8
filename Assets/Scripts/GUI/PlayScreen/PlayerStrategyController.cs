using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStrategyController : MonoBehaviour {

    public Text Name;
    public Dropdown RoleDropdown;
    public Dropdown GroupDropdown;

    private PlayScreenController playScreen;
    private AgentStatus agent;

	public void Initialize(PlayScreenController playScreen, AgentStatus agent)
    {
        this.playScreen = playScreen;
        this.agent = agent;
        NameRefresh();
        RoleDropdown.value = (int)agent.Role;
        GroupDropdown.value = (int)agent.Group;

        RoleDropdown.onValueChanged.AddListener(RoleUpdate);
        GroupDropdown.onValueChanged.AddListener(GroupUpdate);
    }

    void RoleUpdate(int newValue)
    {
        agent.Role = (AgentStatus.RolesEnum)newValue;
    }

    void GroupUpdate(int newValue)
    {
        playScreen.SetGroupForAgent(agent, newValue);
        agent.Group = (AgentStatus.GroupsEnum)newValue;
        NameRefresh();
    }

    void NameRefresh()
    {
        Name.text = agent.Name;
        Name.color = PlayScreenController.GROUP_COLORS[(int)agent.Group];
    }
}
