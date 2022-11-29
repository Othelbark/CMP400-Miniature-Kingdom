using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [SerializeField]
    private InventoryDictionary _inventory;

    [SerializeField] //Temporalily Serialized for testing
    private float _totalInventory = 0.0f; //TODO: add checks to avoid floating point errors desyncing this with _inventory

    [SerializeField]
    private float _capacity = 100.0f;

    private KingdomManager _kingdomManager;

    [SerializeField]
    private Guild _guild = null;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            _kingdomManager = GameObject.FindGameObjectWithTag("KingdomManager").GetComponent<KingdomManager>();
        }
        catch
        {
            Debug.LogError("Can't find kingdom manager.");
        }

        _kingdomManager.AddAgent(this);

        //Add to guild if one is set
        if (_guild != null)
        {
            _guild.AddAgent(this);
        }

        //Update total inventory if inital inventory set
        _totalInventory = 0.0f;
        foreach (KeyValuePair<ResourceType, float> item in _inventory)
        {
            _totalInventory += item.Value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGuild (Guild guild)
    {
        if (_guild != null)
        {
            RemoveFromGuild();
        }

        _guild = guild;
        _guild.AddAgent(this);
    }

    public Guild GetGuild ()
    {
        return _guild;
    }

    public void RemoveFromGuild ()
    {
        if (_guild != null)
        {
            _guild.RemoveAgent(this);
            _guild = null;
        }
    }

    public float GetInventorySpace()
    {
        return Mathf.Max(_capacity - _totalInventory, 0.0f);
    }

    public void ClearInventory()
    {
        _totalInventory = 0.0f;

        _inventory.Clear();
    }

    public float CheckInventoryFor(ResourceType type)
    {
        if (_inventory.ContainsKey(type))
        {
            return _inventory[type];
        }

        return 0;
    }

    // Returns leftover resoucres if capacity is reached
    public float AddToInventory(ResourceType type, float amount)
    {
        if (!_inventory.ContainsKey(type))
        {
            _inventory.Add(type, 0);
        }

        if (amount < _capacity - _totalInventory)
        {
            _inventory[type] += amount;
            _totalInventory += amount;

            return 0;
        }
        else
        {
            float storeableResources = Mathf.Max(_capacity - _totalInventory, 0.0f);

            _inventory[type] += storeableResources;
            _totalInventory += storeableResources;

            return amount - storeableResources;
        }
    }

    //Returns amount actually removed
    public float RemoveFromInventory(ResourceType type, float amount)
    {
        if (!_inventory.ContainsKey(type))
        {
            return 0;
        }

        if (amount < _inventory[type])
        {
            _inventory[type] -= amount;
            _totalInventory -= amount;

            return amount;
        }
        else
        {
            float leftoverResources = _inventory[type];

            _inventory[type] = 0.0f;
            _totalInventory -= leftoverResources;

            return leftoverResources;
        }
    }

}
