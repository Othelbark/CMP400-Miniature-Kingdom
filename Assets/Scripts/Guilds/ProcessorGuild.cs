using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessorGuild : Guild
{
    public void ProcessorGuildConstructor(ResourceProcessor processor, string tag)
    {
        gameObject.tag = tag;

        _processors = new List<ResourceProcessor>();
        _processors.Add(processor);
    }

    protected List<ResourceProcessor> _processors;

    //[SerializeField] // Temp serialised
    protected InventoryDictionary _currentProcessorNeeds;

    [SerializeField]
    [Tooltip("If true agents will always try to fill their invetory when collecting even if total needs are less than the agents capacity.")]
    protected bool _fillInventory = true;

    //Temp TODO: create better system for dealing with multiple casues of inactivity
    protected bool _inactiveForSpace = false;
    protected ResourceType _spaceNeededFor;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        if (_processors.Count <= 0)
        {
            state = GuildState.INACTIVE;
            Debug.LogError("No processor assigned to processor Guild");
        }

        _currentProcessorNeeds = _processors[0].GetNeeds();

        base.Update();
    }


    protected override void SetPriorityName()
    {

        _priorityName = "process";

        foreach (ResourceType type in _processors[0].GetOutputTypes())
        {
            _priorityName += type;
        }
    }


    protected override void UpdateTargetAgentCount()
    {
        base.UpdateTargetAgentCount();
    }
    protected override void ActiveUpdate()
    {
        int agentNumber = -1;
        foreach (Agent agent in _agents)
        {
            agentNumber++;
            if (agentNumber >= _processors.Count)
                break;

            if (agent.state == AgentState.WAITING)
            {

                if (_currentProcessorNeeds.Count == 0)
                {
                    agent.state = AgentState.WORKING;
                    //WORKING -> WAITING
                }
                else if (_processors[agentNumber].HasFinishedGoods())
                {
                    agent.state = AgentState.PICK_UP;
                    //PICK_UP -> STORING -> WAITING
                }
                else if (agent.GetInventorySpace() > 0)
                {
                    agent.state = AgentState.COLLECTING;
                    //COLLECTING -> DROP_OFF -> WAITING
                }
                else
                {
                    agent.state = AgentState.CLEAR_INVENTORY;
                }
            }

            if (agent.state == AgentState.WORKING)
            {
                float distanceToProcessor = (agent.transform.position - _processors[agentNumber].transform.position).magnitude;

                if (distanceToProcessor <= _minInteractionDistance)
                {
                    if (!_processors[agentNumber].Process(Time.deltaTime))
                    {
                        agent.state = AgentState.WAITING;
                    }
                }
                else
                {
                    agent.SetMovingTowards(_processors[agentNumber].transform.position, _minInteractionDistance);
                }
            }
            else if (agent.state == AgentState.PICK_UP)
            {
                float distanceToProcessor = (agent.transform.position - _processors[agentNumber].transform.position).magnitude;

                if (distanceToProcessor <= _minInteractionDistance)
                {
                    foreach (ResourceType type in _processors[agentNumber].GetOutputTypes())
                    {
                        int maxToTake = agent.GetInventorySpace();

                        int amountTaken = _processors[agentNumber].TakeResources(type, maxToTake);

                        agent.AddToInventory(type, amountTaken);
                    }

                    agent.state = AgentState.STORING;
                }
                else
                {
                    agent.SetMovingTowards(_processors[agentNumber].transform.position, _minInteractionDistance);
                }
            }
            else if (agent.state == AgentState.STORING)
            {
                ResourceType typeToStore = ResourceType.NONE;

                foreach (ResourceType type in _processors[agentNumber].GetOutputTypes())
                {
                    if (agent.CheckInventoryFor(type) > 0)
                    {
                        typeToStore = type;
                        break;
                    }
                }


                if (typeToStore != ResourceType.NONE)
                {
                    float distanceToNearestStore;
                    ResourceStore nearestStore = _kingdomManager.NearestResourceStoreOfType(typeToStore, agent.transform.position, out distanceToNearestStore);

                    if (nearestStore == null)
                    {
                        _inactiveForSpace = true;
                        _spaceNeededFor = typeToStore;
                        state = GuildState.INACTIVE;
                    }
                    else if (distanceToNearestStore <= _minStoreDistance)
                    {
                        int fromInventory = agent.RemoveFromInventory(typeToStore);

                        int leftover = nearestStore.AddResources(typeToStore, fromInventory);

                        if (leftover > 0)
                        {
                            agent.AddToInventory(typeToStore, leftover);
                        }
                    }
                    else
                    {
                        agent.SetMovingTowards(nearestStore.transform.position, _minStoreDistance);
                    }
                }
                else
                {
                    agent.state = AgentState.WAITING;
                }
            }
            else if (agent.state == AgentState.COLLECTING)
            {
                ResourceStore nearestStore = null;
                bool needSelected = false;
                ResourceType pickupType = ResourceType.NONE;
                int pickupAmount = 0;

                foreach (KeyValuePair<ResourceType, int> need in _currentProcessorNeeds)
                {
                    if (agent.CheckInventoryFor(need.Key) < need.Value)
                    {
                        needSelected = true;
                        pickupType = need.Key;
                        pickupAmount = need.Value - agent.CheckInventoryFor(need.Key);

                        nearestStore = _kingdomManager.NearestResourceStoreOfType(need.Key, agent.transform.position, true);
                        break;
                    }
                }

                bool processorNeedsAlreadyMeetable = !needSelected;

                #region If _fillInventory collect additional input sets till inventory is full
                if (!needSelected && _fillInventory)
                {
                    InventoryDictionary processorInputs = _processors[agentNumber].GetInputs();
                    int multiplier = 2;
                    while (!needSelected)
                    {
                        foreach (KeyValuePair<ResourceType, int> need in processorInputs)
                        {
                            if (agent.CheckInventoryFor(need.Key) < (need.Value * multiplier))
                            {
                                needSelected = true;
                                pickupType = need.Key;
                                pickupAmount = (need.Value * multiplier) - agent.CheckInventoryFor(need.Key);

                                nearestStore = _kingdomManager.NearestResourceStoreOfType(need.Key, agent.transform.position, true);
                                break;
                            }
                        }
                        multiplier++;
                    }
                }
                #endregion

                if (needSelected)
                {
                    if (nearestStore == null)
                    {
                        //No sources for needed resource
                        if (!processorNeedsAlreadyMeetable)
                            state = GuildState.INACTIVE;
                        else
                            agent.state = AgentState.DROP_OFF;
                    }
                    else
                    {
                        CollectFromStore(agent, nearestStore, pickupType, pickupAmount);
                    }
                }

                if (agent.GetInventorySpace() <= 0 || !needSelected)
                {
                    agent.state = AgentState.DROP_OFF;
                }
            }
            else if (agent.state == AgentState.DROP_OFF)
            {
                float distanceToProcessor = (agent.transform.position - _processors[agentNumber].transform.position).magnitude;

                if (distanceToProcessor <= _minInteractionDistance)
                {
                    foreach (KeyValuePair<ResourceType, int> need in _currentProcessorNeeds)
                    {
                        int fromInventory = agent.RemoveFromInventory(need.Key);

                        int leftover = _processors[agentNumber].AddResources(need.Key, fromInventory);

                        if (leftover > 0)
                        {
                            agent.AddToInventory(need.Key, leftover);
                        }
                    }

                    if (agent.GetCurrentTotalInventory() > 0)
                    {
                        //Inventory has other items in it
                        agent.state = AgentState.CLEAR_INVENTORY;
                    }
                    else
                    {
                        agent.state = AgentState.WAITING;
                    }
                }
                else
                {
                    agent.SetMovingTowards(_processors[agentNumber].transform.position, _minInteractionDistance);
                }
            }


        }
    }
    protected override void InactiveUpdate()
    {
        foreach (ResourceProcessor processor in _processors)
        {
            if (!processor.HasNeeds())
            {
                state = GuildState.ACTIVE;
                return;
            }
        }
        if (_inactiveForSpace)
        {
            if (_kingdomManager.FirstResourceStoreOfType(_spaceNeededFor) != null)
            {
                _inactiveForSpace = false;
                state = GuildState.ACTIVE;
            }
        }
        else
        {
            //determine when there are enough resources to reactivate
            bool enough = true;
            foreach (KeyValuePair<ResourceType, int> need in _currentProcessorNeeds)
            {
                if (need.Value > _kingdomManager.GetTotalResources(need.Key))
                {
                    enough = false;
                    break;
                }
            }
            if (enough)
            {
                state = GuildState.ACTIVE;
            }
        }
    }


    public void AddProcessor(ResourceProcessor processor)
    {
        if (!_processors.Contains(processor))
            _processors.Add(processor);
    }
    public void RemoveProcessor(ResourceProcessor processor)
    {
        if (_processors.Contains(processor))
            _processors.Remove(processor);
    }
}
