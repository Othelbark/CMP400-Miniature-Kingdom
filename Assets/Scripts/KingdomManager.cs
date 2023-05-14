using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KingdomManager : MonoBehaviour
{
    protected List<string> _firstNames = new List<string>() { "Adalhard", "Fabia", "Domitianus", "Dagfinnr", "Romaeus", "Valerius", "Ælfgar", "Thais", "Eudokimos", "Ragnhildr",
                                                                "Lucius", "Rahul", "Berahthram", "Fachtna", "Pippin", "Nonna", "Raimund", "Dubgall", "Hardwin", "Æðelmær",
                                                                "Tressach", "Maximianus", "Felinus", "Dubthach", "Maximilianus", "Lysandros", "Ásgeirr", "Constantia", "Benignus", "Mærwine",
                                                                "Amalia", "Arsaces", "Manegold", "Theoderich", "Walahelin", "Eutychia", "Yseut", "Hallvarðr", "Alfarr", "Airmanagild",
                                                                "Godfrey", "Blanch", "Primula", "Valeria", "Harmon", "Marva", "Clinton", "Gabby", "Totty", "Justin",
                                                                "Torin", "Tasha", "Newt", "Gaby", "Ed", "Collins", "Bertram", "Gorden", "Tarah", "Barney"};
    protected List<string> _lastNames = new List<string>() { "Ecclestone", "Gardener", "Winston", "Kingsley", "Pound", "Walsh", "Norwood", "Dyer", "Ó Cnáimhín", "Montague",
                                                                "Monroe", "Wood", "Fitzpatrick", "Thomas", "MacIntyre", "Fisher", "Mould", "Haines", "Smythe", "Milburn",
                                                                "Leighton", "Grier", "Moon", "Waller", "Ewart", "Huxtable", "Westbrook", "McCaig", "Mooney", " Ó Dochartaigh"};
    protected List<string> _discriptors = new List<string>() { "hard working", "prone to slacking off", "strong", "weak", "the life of the party", "shy", "amicable", "rowdy", "loud", "quiet",
                                                                "holds a grudge", "forgives easily", "forgetful", "perfect memory", "musical", "tone deaf", "content", "ambitious", "arrogant", "humble",
                                                                "brave", "craven", "strong sense of justice", "arbitary", "greedy", "generous", "honest", "deceitful", "calm", "wrathful",
                                                                "diligent", "careless", "compassionate", "callous", "wise", "foolish", "loyal", "untrustworthy", "reliable", "unreliable",
                                                                "cheerful", "depressed", "cunning", "naive", "proud", "self-effacing", "stubborn", "fickle", "polite", "rude",
                                                                "agile", "clumsy", "silly", "serious" };

    [SerializeField] //Temporalily Serialized for testing
    protected List<Guild> _guilds;

    [SerializeField] //Temporalily Serialized for testing
    protected List<Agent> _agents;

    [SerializeField] //Temporalily Serialized for testing
    protected List<Building> _buildings;

    [SerializeField] //Temporalily Serialized for testing
    protected List<ResourceStore> _resourceStores;

    [SerializeField] //Temporalily Serialized for testing
    protected List<Construction> _constructions;

    [SerializeField] //Temporalily Serialized for testing
    protected InventoryDictionary _totalStoredResources;
    [SerializeField] //Temporalily Serialized for testing
    protected InventoryDictionary _totalSpacePerType;


    protected InteractionSystemController _interactionSystemController;
    protected NaturalWorldManager _naturalWorldManager;

    protected Dictionary<string, float> _priorities = new Dictionary<string, float> { { "gatherWOOD", 0.5f }, { "gatherFOOD", 0.5f }, { "gatherSTONE", 0.5f } };

    public float largestCurrentAgentDeficet { get; protected set; } = 0.0f;

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
        _totalSpacePerType.Clear();
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

                if (!_totalSpacePerType.ContainsKey(item.Key))
                {
                    _totalSpacePerType.Add(item.Key, store.GetSpace());
                }
                else
                {
                    _totalSpacePerType[item.Key] += store.GetSpace();
                }
            }
        }
    }

    protected void AgentDistribution()
    {
        //Assign guildless agents to guilds that want agents

        GameObject[] guildlessAgents = GameObject.FindGameObjectsWithTag("Guildless");

        if (guildlessAgents.Length > 0)
        {
            float largestDeficet = 0.0f;
            Guild guildWithLargestDeficet = null;

            foreach (Guild guild in _guilds)
            {
                if (largestDeficet < guild.targetAgentCount - guild.GetCurrentAgentCount())
                {
                    largestDeficet = guild.targetAgentCount - guild.GetCurrentAgentCount();
                    guildWithLargestDeficet = guild;
                }
            }

            if (guildWithLargestDeficet != null)
            {
                guildlessAgents[guildlessAgents.Length - 1].GetComponent<Agent>().SetGuild(guildWithLargestDeficet);
                guildlessAgents = guildlessAgents.Skip<GameObject>(1).ToArray();
            }
            else
            {
                //Debug.Log("No guild found for guildless agent");
            }
        }

        //compute largest remaining deficet
        largestCurrentAgentDeficet = 0.0f;

        foreach (Guild guild in _guilds)
        {
            if (largestCurrentAgentDeficet < guild.targetAgentCount - guild.GetCurrentAgentCount())
            {
                largestCurrentAgentDeficet = guild.targetAgentCount - guild.GetCurrentAgentCount();
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

    public Guild GetGuildByPriorityName(string name)
    {
        foreach (Guild guild in _guilds)
        {
            if (guild._priorityName == name)
            {
                return guild;
            }
        }

        return null;
    }
    public int GetMaxAgentCountInGuilds()
    {
        int maxAgentCount = int.MinValue;
        foreach (Guild guild in _guilds)
        {
            maxAgentCount = Mathf.Max(maxAgentCount, guild.GetCurrentAgentCount());
        }

        return maxAgentCount;
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
    public void AddConstruction(Construction construction)
    {
        _constructions.Add(construction);
    }
    public void RemoveConstruction(Construction construction)
    {
        _constructions.Remove(construction);
    }


    public ResourceStore NearestResourceStoreOfType(ResourceType type, Vector3 position, bool pickupSearch = false, int priorityFilter = -1)
    {
        float shortestDistance = float.MaxValue;
        ResourceStore nearestStore = null;

        //TODO: optimise
        foreach (ResourceStore s in _resourceStores)
        {
            if (s.HasType(type) && ( (s.GetSpace() > 0.0f && !pickupSearch) || (pickupSearch && s.GetAmount(type) > 0) ) && (priorityFilter == -1 || priorityFilter == s.priority) )
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
    public ResourceStore NearestResourceStoreOfType(ResourceType type, Vector3 position, out float shortestDistance, bool pickupSearch = false, int priorityFilter = -1)
    {
        shortestDistance = float.MaxValue;
        ResourceStore nearestStore = null;

        //TODO: optimise
        foreach (ResourceStore s in _resourceStores)
        {
            if (s.HasType(type) && ( (s.GetSpace() > 0.0f && !pickupSearch) || (pickupSearch && s.GetAmount(type) > 0) ) && (priorityFilter == -1 || priorityFilter == s.priority) )
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
    public ResourceStore FirstResourceStoreOfType(ResourceType type, bool pickupSearch = false, int priorityFilter = -1)
    {
        foreach (ResourceStore s in _resourceStores)
        {
            if (s.HasType(type) && ((s.GetSpace() > 0.0f && !pickupSearch) || (pickupSearch && s.GetAmount(type) > 0)) && (priorityFilter == -1 || priorityFilter == s.priority))
            {
                return s;
            }
        }

        return null;
    }
    

    public List<Construction> WaitingForResourcesConstructions()
    {
        List<Construction> waitingConstructions = new List<Construction>();

        foreach (Construction construction in _constructions)
        {
            if (construction.state == ConstructionState.WAITING_FOR_RESOURCES)
                waitingConstructions.Add(construction);
        }

        return waitingConstructions;
    }
    public List<Construction> BuildingConstructions()
    {
        List<Construction> buildingConstructions = new List<Construction>();

        foreach (Construction construction in _constructions)
        {
            if (construction.state == ConstructionState.BUILDING)
                buildingConstructions.Add(construction);
        }

        return buildingConstructions;
    }
    public List<Construction> DeconstructingConstructions()
    {
        List<Construction> deconstructingConstructions = new List<Construction>();

        foreach (Construction construction in _constructions)
        {
            if (construction.state == ConstructionState.DECONSTRUCTING)
                deconstructingConstructions.Add(construction);
        }

        return deconstructingConstructions;
    }
    public List<Construction> WaitingForEmptyConstructions()
    {
        List<Construction> deconstructingConstructions = new List<Construction>();

        foreach (Construction construction in _constructions)
        {
            if (construction.state == ConstructionState.WAITING_FOR_EMPTY)
                deconstructingConstructions.Add(construction);
        }

        return deconstructingConstructions;
    }


    public int GetTotalResources(ResourceType type, bool getLive = false)
    {
        if (getLive)
        {
            int total = 0;
            foreach (ResourceStore store in _resourceStores)
            {
                total += store.GetAmount(type);
            }
            return total;
        }
        else
        {
            if (_totalStoredResources.ContainsKey(type))
            {
                return _totalStoredResources[type];
            }

            return 0;
        }
    }
    public int GetTotalSpaceFor(ResourceType type, bool getLive = false, int priorityFilter = -1)
    {

        if (getLive)
        {
            int total = 0;
            foreach (ResourceStore store in _resourceStores)
            {
                if (store.HasType(type) && (priorityFilter == -1 || priorityFilter == store.priority))
                {
                    total += store.GetSpace();
                }
            }
            return total;
        }
        else
        {
            if (_totalSpacePerType.ContainsKey(type))
            {
                return _totalSpacePerType[type];
            }

            return 0;
        }
    }


    public string GenerateAgentName()
    {
        string name = "";
        name += _firstNames[Random.Range(0, _firstNames.Count)] + " ";
        name += _lastNames[Random.Range(0, _lastNames.Count)];
        return name;
    }
    public string GenerateDescription()
    {
        List<string> discriptions = new List<string>();
        while (discriptions.Count < 3)
        {
            discriptions.Add(_discriptors[Random.Range(0, _discriptors.Count)]);
            discriptions = discriptions.Distinct().ToList();
        }


        string description = "";
        description += discriptions[0] + ", ";
        description += discriptions[1] + ", and ";
        description += discriptions[2];

        char[] chars = description.ToCharArray();
        chars[0] = char.ToUpper(chars[0]);
        return new string(chars);
    }
}
