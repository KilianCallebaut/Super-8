using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayScreenController : MonoBehaviour {

    public static readonly ReadOnlyCollection<string> GROUP_NAMES = new ReadOnlyCollection<string>(new[] {"Albert", "Bob", "Cooper", "Diane", "Ed", "F", "Gordon", "Hawk"});
    public static readonly ReadOnlyCollection<Color> GROUP_COLORS = new ReadOnlyCollection<Color>(new[] {
        new Color(1f, 0.16f, 0.70f), // red
        new Color(1f, 0.85f, 0.70f), // yellow
        new Color(0.51f, 1f, 0.70f), // green
        new Color(0.70f, 1f, 0.85f), // teal
        new Color(0.70f, 0.49f, 1f), // blue
        new Color(0.62f, 0.70f, 1f), // purple
        new Color(1f, 0.70f, 0.71f), // pink
        new Color(0.70f, 0.70f, 0.70f)  // white
    });

    public GameObject CanvasRef;
    public LevelManager LevelManagerRef;
    public GameObject PlayerStrategyPrefab;
    public GameObject GroupStrategyPrefab;
    public GameObject GroupMarkerPrefab;
    public Transform PlayerStrategyContainer;
    public Transform GroupStrategyContainer;
    public Transform GroupMarkerContainer;
    public Button PlayButton;

    [SerializeField]
    private GameObject agentPrefab;
    [SerializeField]
    private GameObject groupPrefab;

    // maybe this is stupid, but it should work, and makes my life a bit easier so whatever
    private List<HashSet<AgentStatus>> groups;
    private Dictionary<int, GroupStrategyController> group2strategy;
    private Dictionary<int, GroupMarkerController> group2marker;
    private List<PlayerStrategyController> playerStrategyControllers;

    private TeamManager teamManager;

    public void Initialize(TeamManager teamManager) {
        this.teamManager = teamManager;
        groups = new List<HashSet<AgentStatus>>();
        group2strategy = new Dictionary<int, GroupStrategyController>();
        group2marker = new Dictionary<int, GroupMarkerController>();
        playerStrategyControllers = new List<PlayerStrategyController>();

        for (int i = 0; i < 8; i++)
        {
            groups.Add(new HashSet<AgentStatus>());
            GameObject groupStrategy = Instantiate(GroupStrategyPrefab);
            GroupStrategyController groupStrategyController = groupStrategy.GetComponent<GroupStrategyController>();
            groupStrategyController.Initialize(this);
            groupStrategyController.transform.SetParent(GroupStrategyContainer);
            group2strategy[i] = groupStrategyController;

            GameObject groupMarker = Instantiate(GroupMarkerPrefab);
            GroupMarkerController groupMarkerController = groupMarker.GetComponent<GroupMarkerController>();
            float containerWidth = GroupMarkerContainer.GetComponent<RectTransform>().rect.width;
            float containerHeight = GroupMarkerContainer.GetComponent<RectTransform>().rect.height;
            groupMarkerController.Initialize(this, i, GroupMarkerContainer, new Vector3(containerWidth / 2, 2 * containerHeight / 3 + i * -35, 0));
            group2marker[i] = groupMarkerController;
        }

        RenderPlayers();

        PlayButton.onClick.AddListener(HitPlay);
	}

    public void SetGroupForAgent(AgentStatus agent, int index)
    {
        // first remove from old group
        int oldIndex = (int)agent.Group;
        groups[oldIndex].Remove(agent);
        UpdateGroupStrategy(oldIndex);

        groups[index].Add(agent);
        UpdateGroupStrategy(index);
    }

    public void UpdateAllGroupStrategies()
    {
        for (int i = 0; i < 8; i++)
        {
            UpdateGroupStrategy(i);
        }
    }

    /**
     * Updates the status of groups, so that the appropriate elements are shown/hidden/updated
     **/
    public void UpdateGroupStrategy(int index)
    {
        group2strategy[index].RefreshGroup(index, groups[index].Count, "");
        group2marker[index].UpdateVisibility(groups[index].Count);
    }

    // gg unity
    // https://issuetracker.unity3d.com/issues/incorrect-auto-layout-on-enabling-slash-disabling-ui-child-prefab-elements-of-vertical-slash-horizontal-layout-group
    public void ForceRefreshGroupHack()
    {
        VerticalLayoutGroup vlg = GroupStrategyContainer.GetComponent<VerticalLayoutGroup>();
        vlg.childControlHeight = !vlg.childControlHeight;
        vlg.childControlHeight = !vlg.childControlHeight;
    }

    // gg unity
    // https://issuetracker.unity3d.com/issues/incorrect-auto-layout-on-enabling-slash-disabling-ui-child-prefab-elements-of-vertical-slash-horizontal-layout-group
    public void ForceRefreshPlayerHack()
    {
        VerticalLayoutGroup vlg = PlayerStrategyContainer.GetComponent<VerticalLayoutGroup>();
        vlg.childControlHeight = !vlg.childControlHeight;
        vlg.childControlHeight = !vlg.childControlHeight;
    }

    public void RenderPlayers()
    {
        // another hacky
        if (groups == null) return;

        for (int i = 0; i < 8; i++)
        {
            groups[i].Clear();
        }

        foreach (PlayerStrategyController psc in playerStrategyControllers)
        {
            Destroy(psc.gameObject);
        }
        playerStrategyControllers.Clear();

        foreach (AgentStatus agent in teamManager.ReadAgents().Where(a => a.Active).ToList())
        {
            groups[(int)agent.Group].Add(agent);
            GameObject playerStrategy = Instantiate(PlayerStrategyPrefab);
            PlayerStrategyController playerStrategyController = playerStrategy.GetComponent<PlayerStrategyController>();
            playerStrategyController.Initialize(this, agent);
            playerStrategy.transform.SetParent(PlayerStrategyContainer);
            playerStrategyControllers.Add(playerStrategyController);
        }

        UpdateAllGroupStrategies();
    }

    public List<Agent> GenerateAgents()
    {
        List<Agent> agentList = new List<Agent>();
        for (int i = 0; i < groups.Count; i++)
        {
            HashSet<AgentStatus> agents = groups[i];
            if (agents.Count == 0) continue;

            GameObject groupObj = Instantiate(groupPrefab);
            Group groupComp = groupObj.AddComponent<Group>();
            groupComp.Initialize();
            groupComp.Objectives.Add(group2marker[i].ToWorldSpace());
            groupComp.name = GROUP_NAMES[i];

            foreach (AgentStatus agentStatus in agents)
            {
                Agent newAgent = agentStatus.CreateAgent(groupComp, agentPrefab);
                newAgent.Team = 1;
                groupComp.AddMember(newAgent);
                agentList.Add(newAgent);
            }
        }

        return agentList;
    }

    void HitPlay()
    {
        LevelManagerRef.RealGo(GenerateAgents());
        CanvasRef.SetActive(false); // yeah
    }
}
