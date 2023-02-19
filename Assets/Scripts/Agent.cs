using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Agent : MonoBehaviour
{
    public AgentState state = AgentState.WAITING;
    protected AgentState _preMovementState = AgentState.WAITING;

    [SerializeField]
    protected InventoryDictionary _inventory;
    [SerializeField] //Temporalily Serialized for testing
    protected float _totalInventory = 0.0f; //TODO: add checks to avoid floating point errors desyncing this with _inventory
    [SerializeField]
    protected float _capacity = 100.0f;

    protected KingdomManager _kingdomManager;
    [SerializeField]
    protected Guild _guild = null;

    protected Vector3 _targetPosition;
    protected float _targetDistance;
    [SerializeField]
    protected float _speed = 1;

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
        else
        {
            gameObject.tag = "Guildless";
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
        // Guild-Independent state logic (MOVING, CLEAR_INVENTORY, DUMP_INVENTORY, etc)
        if (state == AgentState.MOVING)
        {
            Move();
        }
    }

    protected void Move()
    {
        //TODO: pathfinding

        Vector3 towardsTarget = _targetPosition - gameObject.transform.position;
        towardsTarget.Normalize();

        if ((_targetPosition - gameObject.transform.position).magnitude <= Mathf.Max(_speed * Time.deltaTime, _targetDistance))
        {
            state = _preMovementState;

            gameObject.transform.position = _targetPosition - (towardsTarget * (_targetDistance * 0.9f)); //10% closer that target to make sure within range even with floating point errors
        }
        else
        {
            gameObject.transform.Translate(towardsTarget * _speed * Time.deltaTime);
        }
    }

    public void SetMovingTowards(Vector3 pos, float dist)
    {
        _targetPosition = pos;
        _targetDistance = dist;
        _preMovementState = state;
        state = AgentState.MOVING;
    }

    public void SetGuild (Guild guild)
    {
        if (_guild != null)
        {
            RemoveFromGuild();
        }
        gameObject.tag = "Untagged";

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
            state = AgentState.WAITING;

            gameObject.tag = "Guildless";
        }
    }

    public float GetInventorySpace()
    {
        return Mathf.Max(_capacity - _totalInventory, 0.0f);
    }
    public float GetCurrentTotalInventory()
    {
        if (_totalInventory < 0.05f && _totalInventory != 0)
        {
            Debug.LogWarning("Very low inventory, potential floating point error on _totalInventory tracking.");
            return 0;
        }

        return _totalInventory;
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
    //Removes all of a type and returns the amount removed
    public float RemoveFromInventory(ResourceType type)
    {
        float currentResorces = _inventory[type];

        _inventory[type] = 0.0f;
        _totalInventory -= currentResorces;

        return currentResorces;
    }

}
