using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingdomManager : MonoBehaviour
{
    [SerializeField] //Temporalily Serialized for testing
    private List<Guild> _guilds;

    [SerializeField] //Temporalily Serialized for testing
    private List<Agent> _agents;

    [SerializeField] //Temporalily Serialized for testing
    private List<Building> _buildings;

    [SerializeField] //Temporalily Serialized for testing
    private List<SingleResourceStore> _singleResourceStores;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //Assign guildless agents to guilds that want agents
        //TODO: optimise and allow more than one agent to be reasinged per framw.
        GameObject[] guildlessAgents = GameObject.FindGameObjectsWithTag("Guildless");

        if (guildlessAgents.Length > 0)
        {
            foreach (Guild guild in _guilds)
            {
                if (guild.state == GuildState.ACTIVE && guild.GetCurrentAgentCount() < guild.targetAgentCount)
                {
                    guildlessAgents[0].GetComponent<Agent>().SetGuild(guild);
                    break;
                }
            }
        }
    }

    public void AddGuild(Guild guild)
    {
        _guilds.Add(guild);
    }
    public void RemoveGuild(Guild guild)
    {
        _guilds.Remove(guild);
    }

    public void AddAgent(Agent agent)
    {
        _agents.Add(agent);
    }
    public void RemoveAgent(Agent agent)
    {
        _agents.Remove(agent);
    }

    public void AddBuilding(Building building)
    {
        _buildings.Add(building);
    }
    public void RemoveBuilding(Building building)
    {
        _buildings.Remove(building);
    }

    public void AddSingleResourceStore(SingleResourceStore resourceStore)
    {
        _singleResourceStores.Add(resourceStore);
    }
    public void RemoveSingleResourceStore(SingleResourceStore resourceStore)
    {
        _singleResourceStores.Remove(resourceStore);
    }

    public SingleResourceStore NearestSingleResourceStoreOfType(ResourceType type, Vector3 position)
    {
        float shortestDistance = float.MaxValue;
        SingleResourceStore nearestGatherable = null;

        //TODO: optimise
        foreach (SingleResourceStore s in _singleResourceStores)
        {
            if (s.resourceType == type && s.GetSpace() > 0.0f)
            {
                float distance = (s.gameObject.transform.position - position).magnitude;
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestGatherable = s;
                }
            }
        }

        return nearestGatherable;
    }
    public SingleResourceStore FirstSingleResourceStoreOfType(ResourceType type)
    {
        foreach (SingleResourceStore s in _singleResourceStores)
        {
            if (s.resourceType == type && s.GetSpace() > 0.0f)
            {
                return s;
            }
        }

        return null;
    }
}
