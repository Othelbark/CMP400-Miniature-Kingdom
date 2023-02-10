using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStore : Building
{

    [SerializeField]
    [Tooltip("Add entries for every resource this store accepts.")]
    protected InventoryDictionary _currentResorces;

    [SerializeField] //Temporalily Serialized for testing
    protected float _currentResourcesTotal = 0.0f; //TODO: add checks to avoid floating point errors desyncing this with _inventory

    [SerializeField]
    protected float _capacity = 100.0f;


    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        _kingdomManager.AddResourceStore(this);

        //Update current resources total if inital resources set
        _currentResourcesTotal = 0.0f;
        foreach (KeyValuePair<ResourceType, float> item in _currentResorces)
        {
            _currentResourcesTotal += item.Value;
        }
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    public float GetSpace()
    {
        return Mathf.Max(_capacity - _currentResourcesTotal, 0.0f);
    }

    public bool HasType(ResourceType type) { return _currentResorces.ContainsKey(type); }

    // Returns leftover resoucres if capacity is reached
    public float AddResources(ResourceType type, float amount)
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
            float storeableResources = Mathf.Max(_capacity - _currentResourcesTotal, 0.0f);

            _currentResorces[type] += storeableResources;
            _currentResourcesTotal += storeableResources;

            return amount - storeableResources;
        }
    }

    //Returns amount actually taken
    public float TakeResources(ResourceType type, float amount)
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
            float leftoverResources = _currentResorces[type];

            _currentResorces[type] = 0.0f;
            _currentResourcesTotal -= leftoverResources;

            return leftoverResources;
        }
    }
}
