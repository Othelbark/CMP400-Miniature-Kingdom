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
    protected float _buildWork = 0.0f;

    [SerializeField]
    protected float _additionalBuildersScaler = 1.0f;
    protected int _buildCallsSinceLastUpdate = 0;

    //[SerializeField]
    //protected List<Agent> _workers;
    //[SerializeField]
    //protected int _maxBuilders = 1;


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
                    Destroy(gameObject);
                }

                _buildCallsSinceLastUpdate = 0;
                break;
            case ConstructionState.DECONSTRUCTING:
                break;
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
    public InventoryDictionary GetNeeds()
    {
        InventoryDictionary needs = new InventoryDictionary();

        foreach (KeyValuePair<ResourceType, int> potentialNeed in _resourceRequirements)
        {
            if (potentialNeed.Value - _currentResorces[potentialNeed.Key] > 0.0f)
            {
                needs.Add(potentialNeed.Key, potentialNeed.Value - _currentResorces[potentialNeed.Key]);
            }
        }

        return needs;
    }
    public int GetNeedCount() { return GetNeeds().Count; }


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


    //Pass in "false" to cancel a deconstruction
    public void Deconstruct(bool b = true)
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
