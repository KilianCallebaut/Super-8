using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : Singleton<ObjectManager> {

    [SerializeField]
    private GameObject[] Bullets;

    

    //Returns Bullet of this type
    public GameObject getBulletOfType(int type)
    {
        return Bullets[type];
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void setRole(Agent agent, string role)
    {
        var weapon = agent.transform.Find("arm_left/gun").gameObject.AddComponent<Weapon>();
        agent.weapon = weapon;

        switch (role)
        {
            case "Heavy":
                agent.Behaviour = agent.gameObject.AddComponent<HeavyRole>();
                weapon.beHeavyMinigun();
                break;
            case "Assault":
                agent.Behaviour = agent.gameObject.AddComponent<AssaultRole>();
                weapon.beAssaultShotgun();
                break;
            case "Soldier":
                agent.Behaviour = agent.gameObject.AddComponent<SoldierRole>();
                weapon.beSoldierRifle();
                break;
            case "Sniper":
                agent.Behaviour = agent.gameObject.AddComponent<SniperRole>();
                weapon.beSniperRifle();
                break;
            case "Shadow":
                agent.Behaviour = agent.gameObject.AddComponent<ShadowRole>();
                weapon.beShadowPistol();
                break;
            case "Dummy":
                agent.Behaviour = agent.gameObject.AddComponent<DummyRole>();
                break;
            default:
                agent.Behaviour = agent.gameObject.AddComponent<AgentStandardBehaviour>();
                break;
        }
    }

    public static Agent spawnAgent( AgentAttributes attributes, string role)
    {
        var agentSpawn = Instantiate(attributes.agentPrefab);
        Agent agentScript = agentSpawn.AddComponent<Agent>() as Agent;
        setRole(agentScript, role);
        agentScript.Attributes = attributes;
        return agentScript;
    }
    /*
    // Build and add a standard weapon to agent
    public static Weapon AddStandardWeapon(Agent agent)
    {

        var weapon = agent.transform.Find("arm_left/gun").gameObject.AddComponent<Weapon>();
        weapon.projectileType = ObjectManager.Instance.Bullets[0];
        weapon.spread = 0.0f;
        weapon.shotsPerVolley = 10;
        weapon.timeBetweenShots = 0.1f;
        return weapon;
    }
 */
}
