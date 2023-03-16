using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceProcessor : Building
{
    [HideInInspector]
    public ProcessorGuild guild;

    [SerializeField]
    protected InventoryDictionary _processInput;

    [SerializeField]
    protected InventoryDictionary _processOutput;

    protected List<ResourceType> _outputTypes = null;

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

        GameObject guildObject = new GameObject();
        guildObject.name = gameObject.name + "Guild";
        guildObject.transform.SetParent(_kingdomManager.transform);

        guild = guildObject.AddComponent<ProcessorGuild>();
        guild.ProcessorGuildConstructor(this);

        //initalise current resource inventory and tooltip text

        _tooltipText = "Turns";

        foreach (KeyValuePair<ResourceType, int> item in _processInput)
        {
            _tooltipText += " " + item.Value + " ";

            string resourceName = "";
            resourceName += item.Key;
            resourceName = resourceName.Replace("_", " ");
            _tooltipText += resourceName.ToLower();

            _tooltipText += ",";

            if (!_currentResorces.ContainsKey(item.Key))
            {
                _currentResorces.Add(item.Key, 0);
            }
        }

        _tooltipText = _tooltipText.TrimEnd(',');
        _tooltipText += " into";

        foreach (KeyValuePair<ResourceType, int> item in _processOutput)
        {
            _tooltipText += " " + item.Value + " ";

            string resourceName = "";
            resourceName += item.Key;
            resourceName = resourceName.Replace("_", " ");
            _tooltipText += resourceName.ToLower();

            _tooltipText += ",";

            if (!_currentResorces.ContainsKey(item.Key))
            {
                _currentResorces.Add(item.Key, 0);
            }
        }

        _tooltipText = _tooltipText.TrimEnd(',');
        _tooltipText += " every " + _processTime + " seconds";

    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if (_timeThisProcess >= _processTime)
        {
            _timeThisProcess -= _processTime;


            foreach (KeyValuePair<ResourceType, int> item in _processInput)
            {
                _currentResorces[item.Key] -= item.Value;
            }
            foreach (KeyValuePair<ResourceType, int> item in _processOutput)
            {
                _currentResorces[item.Key] += item.Value;
            }

            _processing = false;
        }    
    }

    
    //Returns false when needs are not met
    public bool Process(float dt)
    {
        if (!HasNeeds())
        {
            _processing = true;
            _timeThisProcess += dt;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool HasType(ResourceType type) { return _currentResorces.ContainsKey(type); }

    public int NeedsType(ResourceType type)
    {
        if (!_processInput.ContainsKey(type))
        {
            return 0;
        }
        else
        {
            return Mathf.Max(0, _processInput[type] - _currentResorces[type]);
        }
    }

    public InventoryDictionary GetNeeds()
    {
        InventoryDictionary needs = new InventoryDictionary();

        foreach (KeyValuePair<ResourceType, int> potentialNeed in _processInput)
        {
            if (potentialNeed.Value - _currentResorces[potentialNeed.Key] > 0.0f)
            {
                needs.Add(potentialNeed.Key, potentialNeed.Value - _currentResorces[potentialNeed.Key]);
            }
        }

        return needs;
    }

    public InventoryDictionary GetInputs()
    {
        InventoryDictionary inputs = _processInput;
        return inputs;
    }
    public List<ResourceType> GetOutputTypes()
    {
        if (_outputTypes == null)
        {
            _outputTypes = new List<ResourceType>();
            foreach (KeyValuePair<ResourceType, int> output in _processOutput)
            {
                _outputTypes.Add(output.Key);
            }
        }

        return _outputTypes;
    }

    public bool HasFinishedGoods()
    {
        foreach (ResourceType type in GetOutputTypes())
        {
            if (_currentResorces[type] > 0)
            {
                return true;
            }
        }
        return false;
    }

    public bool HasNeeds()
    {
        foreach (KeyValuePair<ResourceType, int> potentialNeed in _processInput)
        {
            if (potentialNeed.Value - _currentResorces[potentialNeed.Key] > 0.0f)
            {
                return true;
            }
        }
        return false;
    }

    // Returns leftover resoucres if capacity is reached
    public int AddResources(ResourceType type, int amount)
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
    public int TakeResources(ResourceType type, int amount)
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
            int leftoverResources = _currentResorces[type];

            _currentResorces[type] = 0;

            return leftoverResources;
        }
    }

    public override string GetText(string additionalText = "")
    {
        string processorInfo = "";

        if (_processing)
        {
            processorInfo += "\n";
            //Percent done process
            int processPercent = Mathf.FloorToInt((_timeThisProcess / _processTime) * 100);
            processorInfo += "Processing: " + processPercent + "%";
        }


        foreach (KeyValuePair<ResourceType, int> item in _currentResorces)
        {
            if (item.Value > 0)
            {
                processorInfo += "\n";
                processorInfo += "Current ";

                string resourceName = "";
                resourceName += item.Key;
                resourceName = resourceName.Replace("_", " ");
                processorInfo += resourceName.ToLower();

                processorInfo += ": " + item.Value;
            }
        }

        return base.GetText(processorInfo);
    }
}
