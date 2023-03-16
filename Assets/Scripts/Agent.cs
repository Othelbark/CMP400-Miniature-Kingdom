using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Agent : TooltipedObject
{
    public AgentState state = AgentState.WAITING;
    protected AgentState _preMovementState = AgentState.WAITING;

    [SerializeField]
    protected InventoryDictionary _inventory;
    [SerializeField] //Temporalily Serialized for testing
    protected int _totalInventory = 0;
    [SerializeField]
    protected int _capacity = 100;

    protected KingdomManager _kingdomManager;
    [SerializeField]
    protected Guild _guild = null;

    protected Vector3 _targetPosition;
    protected float _targetDistance;
    [SerializeField]
    protected float _speed = 1;

    protected float _residualWork = 0.0f;

    //[SerializeField]
    //protected ContactFilter2D _collisionFilter;

    protected ResourceStore _targetStore = null;
    public Gatherable targetGatherable { get; protected set; } = null;

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
        _tooltipName = _kingdomManager.GenerateAgentName();
        _tooltipText = _kingdomManager.GenerateDescription();

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
        _totalInventory = 0;
        foreach (KeyValuePair<ResourceType, int> item in _inventory)
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
            MoveState();
        }
        else if (state == AgentState.CLEAR_INVENTORY)
        {
            ClearInventoryState();
        }
        else if (state == AgentState.DUMP_INVENTORY)
        {
            DumpInvetoryState();
        }

        //if not collecting then reset any target gathererable
        if (state != AgentState.COLLECTING && _preMovementState != AgentState.COLLECTING && targetGatherable != null)
        {
            targetGatherable.RemoveGatherer(this);
            targetGatherable = null;
        }
        if (targetGatherable != null)
        {
            if (targetGatherable.state == GatherableState.NON_GATHERABLE)
            {
                targetGatherable.RemoveGatherer(this);
                targetGatherable = null;
            }
        }
    }

    protected void MoveState()
    {
        Vector3 towardsTarget;
        towardsTarget = _targetPosition - gameObject.transform.position;
        towardsTarget.Normalize();

        //TODO: Pathfinding
        //List<RaycastHit2D> raycastHits = new List<RaycastHit2D>();
        //if (Physics2D.Raycast(transform.position, towardsTarget, _collisionFilter, raycastHits) > 0)
        //{
        //}

        if ((_targetPosition - gameObject.transform.position).magnitude <= Mathf.Max(_speed * Time.deltaTime, _targetDistance))
        {
            state = _preMovementState;

            gameObject.transform.position = _targetPosition - (towardsTarget * (_targetDistance * 0.9f)); //Warp to target position if agent would overshoot
        }
        else
        {
            gameObject.transform.Translate(towardsTarget * (_speed * Time.deltaTime));
        }

    }

    protected void ClearInventoryState()
    {
        if (_targetStore != null)
        {
            ResourceType typeToStore = ResourceType.NONE;

            foreach (KeyValuePair<ResourceType, int> item in _inventory)
            {
                if (item.Value > 0 && _targetStore.HasType(item.Key))
                {
                    typeToStore = item.Key;
                    break;
                }
            }


            if (typeToStore != ResourceType.NONE)
            {
                float distanceToTargetStore = (_targetStore.transform.position - transform.position).magnitude;

                if (distanceToTargetStore <= 0)
                {
                    int fromInventory = RemoveFromInventory(typeToStore);

                    int leftover = _targetStore.AddResources(typeToStore, fromInventory);

                    if (leftover > 0)
                    {
                        AddToInventory(typeToStore, leftover);
                    }
                }
                else
                {
                    SetMovingTowards(_targetStore.transform.position, 0);
                }
            }
            else
            {
                state = AgentState.WAITING;
                SetMovingTowards(_targetStore.transform.position, 0.0f);
                _targetStore = null;
            }
        }
        else
        {
            ResourceType typeToStore = ResourceType.NONE;

            foreach (KeyValuePair<ResourceType, int> item in _inventory)
            {
                if (item.Value > 0)
                {
                    typeToStore = item.Key;
                    break;
                }
            }

            if (typeToStore != ResourceType.NONE)
            {
                float distanceToNearestStore;
                ResourceStore nearestStore;
                nearestStore = _kingdomManager.NearestResourceStoreOfType(typeToStore, gameObject.transform.position, out distanceToNearestStore);

                if (nearestStore == null)
                {
                    RemoveFromInventory(typeToStore);
                }
                else if (distanceToNearestStore <= 0)
                {
                    int fromInventory = RemoveFromInventory(typeToStore);

                    int leftover = nearestStore.AddResources(typeToStore, fromInventory);

                    if (leftover > 0)
                    {
                        AddToInventory(typeToStore, leftover);
                    }
                }
                else
                {
                    SetMovingTowards(nearestStore.transform.position, 0);
                }
            }
            else
            {
                state = AgentState.WAITING;
            }
        }
    }
    protected void DumpInvetoryState()
    {
        ClearInventory();
        state = AgentState.WAITING;
    }

    public void SetMovingTowards(Vector3 pos, float dist, bool resetState = false)
    {
        _targetPosition = pos;
        _targetDistance = dist;

        if (resetState || state == AgentState.MOVING)
            _preMovementState = AgentState.WAITING;
        else
            _preMovementState = state;

        state = AgentState.MOVING;
    }

    public void ClearInventoryInto(ResourceStore store)
    {

        _targetStore = store;
        state = AgentState.CLEAR_INVENTORY;
    }

    public bool SetTargetGatherable(Gatherable gatherable, bool forceAdd = false)
    {
        if (targetGatherable != null)
        {
            targetGatherable.RemoveGatherer(this);
        }

        if (gatherable == null)
        {
            targetGatherable = null;
            return true;
        }

        if (gatherable.AddGatherer(this, forceAdd))
        {
            targetGatherable = gatherable;
            return true;
        }
        return false;
    }
    //only to be used when gatherabel is spent
    public void ClearTargetGatherable()
    {
        targetGatherable = null;
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

    public void SetResidualWork(float rw) { _residualWork = rw; }
    public float GetResidualWork() { return _residualWork; }

    public int GetInventorySpace()
    {
        return Mathf.Max(_capacity - _totalInventory, 0);
    }
    public int GetCurrentTotalInventory()
    {
        if (_totalInventory < 0)
        {
            Debug.LogWarning("Negitive total inventory");
            return 0;
        }

        return _totalInventory;
    }
    public void ClearInventory()
    {
        _totalInventory = 0;

        _inventory.Clear();
    }
    public int CheckInventoryFor(ResourceType type)
    {
        if (_inventory.ContainsKey(type))
        {
            return _inventory[type];
        }

        return 0;
    }
    // Returns leftover resoucres if capacity is reached
    public int AddToInventory(ResourceType type, int amount)
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
            int storeableResources = Mathf.Max(_capacity - _totalInventory, 0);

            _inventory[type] += storeableResources;
            _totalInventory += storeableResources;

            return amount - storeableResources;
        }
    }
    //Returns amount actually removed
    public int RemoveFromInventory(ResourceType type, int amount)
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
            int leftoverResources = _inventory[type];

            _inventory[type] = 0;
            _totalInventory -= leftoverResources;

            return leftoverResources;
        }
    }
    //Removes all of a type and returns the amount removed
    public int RemoveFromInventory(ResourceType type)
    {
        int currentResorces = _inventory[type];

        _inventory[type] = 0;
        _totalInventory -= currentResorces;

        return currentResorces;
    }

    public override string GetText(string additionalText = "")
    {
        string agentInfo = "";
        foreach (KeyValuePair<ResourceType, int> item in _inventory)
        {
            if (item.Value > 0)
            {
                agentInfo += "\n";
                agentInfo += "Current ";

                string resourceName = "";
                resourceName += item.Key;
                resourceName = resourceName.Replace("_", " ");
                agentInfo += resourceName.ToLower();

                agentInfo += ": " + item.Value;
            }
        }


        agentInfo += "\n";
        agentInfo += "Inventory: " + _totalInventory + "/" + _capacity;

        return base.GetText(agentInfo);
    }

}
