using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingdomManager : MonoBehaviour
{
    [SerializeField] //Temporalily Serialized for testing
    protected List<Guild> _guilds;

    [SerializeField] //Temporalily Serialized for testing
    protected List<Agent> _agents;

    [SerializeField] //Temporalily Serialized for testing
    protected List<Building> _buildings;

    [SerializeField] //Temporalily Serialized for testing
    protected List<ResourceStore> _resourceStores;

    [SerializeField] //Temporalily Serialized for testing
    protected InventoryDictionary _totalStoredResources;


    protected InteractionSystemController _interactionSystemController;
    protected NaturalWorldManager _naturalWorldManager;

    protected Dictionary<string, float> _priorities = new Dictionary<string, float> { { "gatherWOOD", 0.5f }, { "gatherFOOD", 0.5f }, { "gatherSTONE", 0.5f }, { "process", 0.5f } };

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            _interactionSystemController = GameObject.FindGameObjectWithTag("InteractionSystemController").GetComponent<InteractionSystemController>();
        }
        catch
        {
            Debug.LogError("Can't find interaction system controller.");
        }
        try
        {
            _naturalWorldManager = GameObject.FindGameObjectWithTag("NaturalWorldManager").GetComponent<NaturalWorldManager>();
        }
        catch
        {
            Debug.LogError("Can't find natural world manager.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (_interactionSystemController.GetControlType() == ControlType.ABSTRACTED)
        {
            //Automatic management
            AgentDistribution();
        }


        //Total up all stored resources
        _totalStoredResources.Clear();
        foreach(ResourceStore store in _resourceStores)
        {
            foreach (KeyValuePair<ResourceType, int> item in store.GetResources())
            {
                if (!_totalStoredResources.ContainsKey(item.Key))
                {
                    _totalStoredResources.Add(item.Key, item.Value);
                }
                else
                {
                    _totalStoredResources[item.Key] += item.Value;
                }
            }
        }
    }

    protected void AgentDistribution()
    {
        //Assign guildless agents to guilds that want agents
        //TODO: optimise- and allow more than one agent to be reasinged per frame?
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

    public void SetPriority(string name, float value)
    {
        if (_priorities.ContainsKey(name))
        {
            _priorities[name] = value;
        }
        else
        {
            _priorities.Add(name, value);
        }
    }

    public float GetPriority(string name)
    {

        if (_priorities.ContainsKey(name))
        {
            return _priorities[name];
        }
        else
        {
            return 0;
        }
    }

    public float GetTotalPriority()
    {
        float total = 0;
        foreach (KeyValuePair<string, float> priority in _priorities)
        {
            total += priority.Value;
        }

        return Mathf.Max(total, 1.0f);
    }

    public void AddGuild(Guild guild)
    {
        _guilds.Add(guild);
    }
    public void RemoveGuild(Guild guild)
    {
        _guilds.Remove(guild);
    }

    public List<Guild> GetGuilds()
    {
        List<Guild> guilds = _guilds;
        return guilds;
    }

    public void AddAgent(Agent agent)
    {
        _agents.Add(agent);
    }
    public void RemoveAgent(Agent agent)
    {
        _agents.Remove(agent);
    }

    public int GetAgentCount()
    {
        return _agents.Count;
    }

    public void AddBuilding(Building building)
    {
        _buildings.Add(building);
    }
    public void RemoveBuilding(Building building)
    {
        _buildings.Remove(building);
    }
   
    public void AddResourceStore(ResourceStore resourceStore)
    {
        _resourceStores.Add(resourceStore);
    }
    public void RemoveResourceStore(ResourceStore resourceStore)
    {
        _resourceStores.Remove(resourceStore);
    }

    public ResourceStore NearestResourceStoreOfType(ResourceType type, Vector3 position, bool pickupSearch = false)
    {
        float shortestDistance = float.MaxValue;
        ResourceStore nearestStore = null;

        //TODO: optimise
        foreach (ResourceStore s in _resourceStores)
        {
            if (s.HasType(type) && ( (s.GetSpace() > 0.0f && !pickupSearch) || (pickupSearch && s.GetAmount(type) > 0) ) )
            {
                float distance = (s.gameObject.transform.position - position).magnitude;
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestStore = s;
                }
            }
        }

        return nearestStore;
    }
    public ResourceStore NearestResourceStoreOfType(ResourceType type, Vector3 position, out float shortestDistance, bool pickupSearch = false)
    {
        shortestDistance = float.MaxValue;
        ResourceStore nearestStore = null;

        //TODO: optimise
        foreach (ResourceStore s in _resourceStores)
        {
            if (s.HasType(type) && ( (s.GetSpace() > 0.0f && !pickupSearch) || (pickupSearch && s.GetAmount(type) > 0) ) )
            {
                float distance = (s.gameObject.transform.position - position).magnitude;
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestStore = s;
                }
            }
        }

        return nearestStore;
    }
    public ResourceStore FirstResourceStoreOfType(ResourceType type)
    {
        foreach (ResourceStore s in _resourceStores)
        {
            if (s.HasType(type) && s.GetSpace() > 0.0f)
            {
                return s;
            }
        }

        return null;
    }
    
    public int GetTotalResources(ResourceType type)
    {

        if (_totalStoredResources.ContainsKey(type))
        {
            return _totalStoredResources[type];
        }

        return 0;
    }
}
