using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceProcessor : Building
{
    [SerializeField]
    protected InventoryDictionary _processInput;

    [SerializeField]
    protected InventoryDictionary _processOutput;

    [SerializeField] //Temporalily Serialized for testing
    protected InventoryDictionary _currentResorces;

    [SerializeField]
    protected float _processTime = 5.0f;
    protected float _timeThisProcess = 0.0f;

    protected bool _processing = false;


    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        //initalise current resource inventory
        foreach (KeyValuePair<ResourceType, float> item in _processInput)
        {
            if (!_currentResorces.ContainsKey(item.Key))
            {
                _currentResorces.Add(item.Key, 0.0f);
            }
        }
        foreach (KeyValuePair<ResourceType, float> item in _processOutput)
        {
            if (!_currentResorces.ContainsKey(item.Key))
            {
                _currentResorces.Add(item.Key, 0.0f);
            }
        }
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if (_timeThisProcess >= _processTime)
        {
            _timeThisProcess -= _processTime;


            foreach (KeyValuePair<ResourceType, float> item in _processInput)
            {
                _currentResorces[item.Key] -= item.Value;
            }
            foreach (KeyValuePair<ResourceType, float> item in _processOutput)
            {
                _currentResorces[item.Key] += item.Value;
            }

            _processing = false;
        }    
    }

    //TODO: make this not work if called more than once in a frame (generally make multiple agents per guild work better)
    public void Process(float dt)
    {
        if (!HasNeeds())
        {
            _processing = true;
            _timeThisProcess += dt;
        }
    }

    public bool HasType(ResourceType type) { return _currentResorces.ContainsKey(type); }

    public float NeedsType(ResourceType type)
    {
        if (!_processInput.ContainsKey(type))
        {
            return 0.0f;
        }
        else
        {
            return Mathf.Max(0.0f, _processInput[type] - _currentResorces[type]);
        }
    }

    public InventoryDictionary GetNeeds()
    {
        InventoryDictionary needs = new InventoryDictionary();

        foreach (KeyValuePair<ResourceType, float> potentialNeed in _processInput)
        {
            if (potentialNeed.Value - _currentResorces[potentialNeed.Key] > 0.0f)
            {
                needs.Add(potentialNeed.Key, potentialNeed.Value - _currentResorces[potentialNeed.Key]);
            }
        }

        return needs;
    }

    protected bool HasNeeds()
    {
        foreach (KeyValuePair<ResourceType, float> potentialNeed in _processInput)
        {
            if (potentialNeed.Value - _currentResorces[potentialNeed.Key] > 0.0f)
            {
                return true;
            }
        }
        return false;
    }

    // Returns leftover resoucres if capacity is reached
    public float AddResources(ResourceType type, float amount)
    {
        if (!_processInput.ContainsKey(type))
        {
            //can't take resources of this type
            Debug.LogWarning("Trying to add resources to a processor that doesn't take that type.");
            return amount;
        }

        _currentResorces[type] += amount;
        return 0;
    }

    //Returns amount actually taken
    public float TakeResources(ResourceType type, float amount)
    {
        if (_processing)
        {
            if (!_processOutput.ContainsKey(type))
            {
                //trying to take non-output resources while processing
                Debug.LogWarning("Trying to take non-output resources from a processor while processing.");
                return 0;
            }
        }

        if (!_currentResorces.ContainsKey(type))
        {
            //never has resources of this type
            Debug.LogWarning("Trying to take resources from a processor that never holds that type.");
            return 0;
        }

        if (amount < _currentResorces[type])
        {
            _currentResorces[type] -= amount;

            return amount;
        }
        else
        {
            float leftoverResources = _currentResorces[type];

            _currentResorces[type] = 0.0f;

            return leftoverResources;
        }
    }
}
