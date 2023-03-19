using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construction : Building
{
    public ConstructionState state = ConstructionState.WAITING_FOR_RESOURCES;

    [SerializeField]
    protected GameObject _buildingPrefab;

    [SerializeField]
    protected InventoryDictionary _resourceRequirements;

    [SerializeField] //Temporalily Serialized for testing
    protected InventoryDictionary _currentResorces;

    [SerializeField]
    protected float _buildTime = 20.0f;
    [SerializeField]
    [Tooltip("Muliplier applied to work when undoing BuildTime")]
    protected float _deconstructBuildWorkMultiplier = 2.0f;
    protected float _buildWork = 0.0f;

    [SerializeField]
    protected float _additionalBuildersScaler = 0.7f;
    protected int _buildCallsSinceLastUpdate = 0;

    [SerializeField]
    protected int _maxBuilders = 1;


    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        _kingdomManager.AddConstruction(this);

        foreach (KeyValuePair<ResourceType, int> item in _resourceRequirements)
        {
            if (!_currentResorces.ContainsKey(item.Key))
            {
                _currentResorces.Add(item.Key, 0);
            }
        }
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        

        switch (state)
        {
            case ConstructionState.WAITING_FOR_RESOURCES:
                if (!HasNeeds())
                {
                    state = ConstructionState.BUILDING;
                }
                break;
            case ConstructionState.BUILDING:
                if (_buildWork >= _buildTime)
                {
                    GameObject finishedBuilding = Instantiate(_buildingPrefab, _kingdomManager.transform);
                    finishedBuilding.transform.position = transform.position;

                    _kingdomManager.RemoveConstruction(this);
                    foreach (Agent agent in _assignedAgents)
                    {
                        agent.ClearTargetBuilding();
                    }
                    Destroy(gameObject);
                }

                _buildCallsSinceLastUpdate = 0;
                break;
            case ConstructionState.DECONSTRUCTING:
                if (_buildWork <= 0.0f)
                {
                    state = ConstructionState.WAITING_FOR_EMPTY;
                }
                break;
            case ConstructionState.WAITING_FOR_EMPTY:
                if (GetTotalResources() <= 0)
                {
                    _kingdomManager.RemoveConstruction(this);
                    foreach (Agent agent in _assignedAgents)
                    {
                        agent.ClearTargetBuilding();
                    }
                    Destroy(gameObject);
                }
                break;
        }
    }
    
    
    public override int GetMaxAssignedAgents()
    {
        if (state == ConstructionState.BUILDING)
        {
            return _maxBuilders;
        }
        else if (state == ConstructionState.WAITING_FOR_RESOURCES)
        {
            float fMaxAgents = (float)GetTotalFillableNeeds() / (float)Constants.AgentInventorySpace;
            int iMaxAgents = Mathf.CeilToInt(fMaxAgents);
            return iMaxAgents;
        }
        else if (state == ConstructionState.DECONSTRUCTING)
        {
            return _maxBuilders;
        }
        else // if (state == ConstructionState.WAITING_FOR_EMPTY)
        {
            float fMaxAgents = (float)GetTotalStoreableResources() / (float)Constants.AgentInventorySpace;
            int iMaxAgents = Mathf.CeilToInt(fMaxAgents);
            return iMaxAgents;
        }
    }


    public bool Build(float dt)
    {
        if (state == ConstructionState.BUILDING)
        {
            _buildWork += dt * Mathf.Pow(_additionalBuildersScaler, _buildCallsSinceLastUpdate);

            _buildCallsSinceLastUpdate++; 
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool Deconstruct(float dt)
    {
        if(state == ConstructionState.DECONSTRUCTING)
        {
            _buildWork -= dt * _deconstructBuildWorkMultiplier * Mathf.Pow(_additionalBuildersScaler, _buildCallsSinceLastUpdate);

            _buildCallsSinceLastUpdate++;
            return true;
        }
        else
        {
            return false;
        }
    }


    public bool HasNeeds()
    {
        foreach (KeyValuePair<ResourceType, int> potentialNeed in _resourceRequirements)
        {
            if (potentialNeed.Value - _currentResorces[potentialNeed.Key] > 0.0f)
            {
                return true;
            }
        }
        return false;
    }
    public InventoryDictionary GetFillableNeeds(bool excludeHeldResources = false)
    {
        InventoryDictionary needs = new InventoryDictionary();

        foreach (KeyValuePair<ResourceType, int> resource in _resourceRequirements)
        {
            if (excludeHeldResources)
            {
                if (resource.Value - _currentResorces[resource.Key] - GetTotalResourcesInAssignedAgents(resource.Key) > 0.0f)
                {
                    int potentialNeed = resource.Value - _currentResorces[resource.Key] - GetTotalResourcesInAssignedAgents(resource.Key);
                    needs.Add(resource.Key, Mathf.Min(potentialNeed, _kingdomManager.GetTotalResources(resource.Key)));
                }
            }
            else
            {
                if (resource.Value - _currentResorces[resource.Key] > 0.0f)
                {
                    int potentialNeed = resource.Value - _currentResorces[resource.Key];
                    needs.Add(resource.Key, Mathf.Min(potentialNeed, _kingdomManager.GetTotalResources(resource.Key) + GetTotalResourcesInAssignedAgents(resource.Key)));
                }
            }
        }

        return needs;
    }
    public int GetTotalFillableNeeds()
    {

        int totalNeeds = 0;

        foreach (KeyValuePair<ResourceType, int> resource in _resourceRequirements)
        {
            if (resource.Value - _currentResorces[resource.Key] > 0.0f)
            {
                int potentialNeed = resource.Value - _currentResorces[resource.Key];
                totalNeeds += Mathf.Min(potentialNeed, _kingdomManager.GetTotalResources(resource.Key) + GetTotalResourcesInAssignedAgents(resource.Key));
            }
        }

        return totalNeeds;
    }


    protected int GetTotalResourcesInAssignedAgents(ResourceType type)
    {
        int total = 0;

        foreach (Agent agent in _assignedAgents)
        {
            total += agent.CheckInventoryFor(type);
        }

        return total;
    }


    public int AddResources(ResourceType type, int amount)
    {
        if (!_resourceRequirements.ContainsKey(type))
        {
            //can't take resources of this type
            Debug.LogWarning("Trying to add resources to a construction that doesn't take that type.");
            return amount;
        }

        if (state != ConstructionState.WAITING_FOR_RESOURCES)
        {
            Debug.LogWarning("Trying to add resources to a construction in " + state + " state.");
            return amount;
        }

        if (_resourceRequirements[type] - _currentResorces[type] > amount)
        {
            _currentResorces[type] += amount;
            return 0;
        }
        else
        {
            int leftover = amount - (_resourceRequirements[type] - _currentResorces[type]);
            _currentResorces[type] = _resourceRequirements[type];
            return leftover;
        }
    }
    public int TakeResources(ResourceType type, int amount)
    {

        if (!_currentResorces.ContainsKey(type))
        {
            //never has resources of this type
            Debug.LogWarning("Trying to take resources from a construction that never holds that type.");
            return 0;
        }

        if (state != ConstructionState.DECONSTRUCTING)
        {
            Debug.LogWarning("Trying to take resources from a construction in " + state + " state.");
            return amount;
        }

        if (amount < _currentResorces[type])
        {
            _currentResorces[type] -= amount;

            return amount;
        }
        else
        {
            int leftoverResources = _currentResorces[type];

            _currentResorces[type] = 0;

            return leftoverResources;
        }
    }
    public int GetTotalStoreableResources()
    {
        int total = 0;

        foreach (KeyValuePair<ResourceType, int> resource in _currentResorces)
        {
            total += Mathf.Max(resource.Value + GetTotalResourcesInAssignedAgents(resource.Key), _kingdomManager.GetTotalSpaceFor(resource.Key));
        }

        return (total);
    }
    public int GetTotalResources()
    {
        int total = 0;

        foreach (KeyValuePair<ResourceType, int> resource in _currentResorces)
        {
            total += resource.Value;
        }

        return (total);
    }
    public InventoryDictionary GetCurrentResources()
    {
        return _currentResorces;
    }


    //Pass in "false" to cancel a deconstruction
    public void SetDeconstruct(bool b = true)
    {
        if (b)
        {
            state = ConstructionState.DECONSTRUCTING;
        }
        else if (state == ConstructionState.DECONSTRUCTING)
        {
            state = ConstructionState.WAITING_FOR_RESOURCES;
        }
    }


    public override string GetText(string additionalText = "")
    {
        string buildingInfo = "";

        if (state == ConstructionState.BUILDING)
        {
            buildingInfo += "\n";
            //Percent done nuilding
            int buildingPercent = Mathf.FloorToInt((_buildWork / _buildTime) * 100);
            buildingInfo += "Building: " + buildingPercent + "%";
        }


        foreach (KeyValuePair<ResourceType, int> item in _currentResorces)
        {
            buildingInfo += "\n";

            string resourceName = "";
            resourceName += item.Key;
            resourceName = resourceName.Replace("_", " ");
            resourceName = resourceName.ToLower();
            char[] resourceNameChars = resourceName.ToCharArray();
            resourceNameChars[0] = char.ToUpper(resourceNameChars[0]);
            buildingInfo += new string(resourceNameChars);

            buildingInfo += ": " + item.Value;
            buildingInfo += "/" + _resourceRequirements[item.Key];
        }

        return base.GetText(buildingInfo);
    }

}
