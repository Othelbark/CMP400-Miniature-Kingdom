using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStore : Building
{

    [SerializeField]
    [Tooltip("Add entries for every resource this store accepts.")]
    protected InventoryDictionary _currentResorces;

    [SerializeField] //Temporalily Serialized for testing
    protected int _currentResourcesTotal = 0; 

    [SerializeField]
    protected int _capacity = 100;


    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        _kingdomManager.AddResourceStore(this);

        //Update current resources total if inital resources set
        _currentResourcesTotal = 0;
        foreach (KeyValuePair<ResourceType, int> item in _currentResorces)
        {
            _currentResourcesTotal += item.Value;
        }
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    public int GetSpace()
    {
        return Mathf.Max(_capacity - _currentResourcesTotal, 0);
    }

    public bool HasType(ResourceType type) { return _currentResorces.ContainsKey(type); }

    public int GetAmount(ResourceType type) 
    {
        if (HasType(type))
        {
            return _currentResorces[type];
        }
        else
        {
            return 0;
        }
    }

    public InventoryDictionary GetResources()
    {
        return _currentResorces;
    }

    // Returns leftover resoucres if capacity is reached
    public int AddResources(ResourceType type, int amount)
    {
        if (!_currentResorces.ContainsKey(type))
        {
            //can't take resources of this type
            Debug.LogWarning("Trying to add resources to a store that can't take that type.");
            return amount;
        }

        if ((_capacity - _currentResourcesTotal) >= amount)
        {
            _currentResorces[type] += amount;
            _currentResourcesTotal += amount;
            return 0;
        }
        else
        {
            int storeableResources = Mathf.Max(_capacity - _currentResourcesTotal, 0);

            _currentResorces[type] += storeableResources;
            _currentResourcesTotal += storeableResources;

            return amount - storeableResources;
        }
    }

    //Returns amount actually taken
    public int TakeResources(ResourceType type, int amount)
    {
        if (!_currentResorces.ContainsKey(type))
        {
            //never has resources of this type
            Debug.LogWarning("Trying to take resources from a store that doesn't store that type.");
            return 0;
        }

        if (amount < _currentResorces[type])
        {
            _currentResorces[type] -= amount;
            _currentResourcesTotal -= amount;

            return amount;
        }
        else
        {
            int leftoverResources = _currentResorces[type];

            _currentResorces[type] = 0;
            _currentResourcesTotal -= leftoverResources;

            return leftoverResources;
        }
    }
}
