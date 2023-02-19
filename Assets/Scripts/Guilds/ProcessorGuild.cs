using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessorGuild : Guild
{

    [SerializeField]
    protected ResourceProcessor _processor;

    //[SerializeField] // Temp serialised
    protected InventoryDictionary _currentProcessorNeeds;

    [SerializeField]
    protected float _minProcessingDistance = 0.2f;
    [SerializeField]
    protected float _minPickupDistance = 0.0f;

    [SerializeField]
    [Tooltip("If true agents will always try to fill their invetory when collecting even if total needs are less than the agents capacity.")]
    protected bool _fillInventory = false;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        targetAgentCount = 1;
    }

    // Update is called once per frame
    new void Update()
    {
        if (_agents.Count > 1)
        {
            Debug.LogError("More than one agent assigned to a processor Guild");
        }
        if (!_processor)
        {
            state = GuildState.INACTIVE;
            Debug.LogError("No processor assigned to processor Guild");
        }

        _currentProcessorNeeds = _processor.GetNeeds();

        if (state == GuildState.ACTIVE && _agents.Count > 0)
        {
            if (_agents[0].state == AgentState.WAITING)
            {

                if (_currentProcessorNeeds.Count == 0)
                {
                    _agents[0].state = AgentState.WORKING;
                }
                else if (_agents[0].GetInventorySpace() > 0)
                {
                    _agents[0].state = AgentState.COLLECTING;
                }
                else
                {
                    _agents[0].state = AgentState.STORING;
                }
            }

            if (_agents[0].state == AgentState.WORKING)
            {
                float distanceToProcessor = (_agents[0].transform.position - _processor.transform.position).magnitude;

                if (distanceToProcessor <= _minProcessingDistance)
                {
                    if(!_processor.Process(Time.deltaTime))
                    {
                        _agents[0].state = AgentState.WAITING;
                    }
                }
                else
                {
                    _agents[0].SetMovingTowards(_processor.transform.position, _minProcessingDistance);
                }
            }
            else if (_agents[0].state == AgentState.COLLECTING)
            {
                float distanceToNearestStore = float.MaxValue;
                ResourceStore nearestStore = null;

                bool needSelected = false;
                ResourceType pickupType = ResourceType.NONE;
                float pickupAmount = 0.0f;

                foreach (KeyValuePair<ResourceType, float> need in _currentProcessorNeeds)
                {
                    if (_agents[0].CheckInventoryFor(need.Key) < need.Value)
                    {
                        needSelected = true;
                        pickupType = need.Key;
                        pickupAmount = need.Value - _agents[0].CheckInventoryFor(need.Key);

                        nearestStore = _kingdomManager.NearestResourceStoreOfType(need.Key, _agents[0].transform.position, out distanceToNearestStore, true);
                        break;
                    }
                }

                bool processorNeedsAlreadyMeetable = !needSelected;

                #region If _fillInventory collect additional input sets till inventory is full
                if (!needSelected && _fillInventory)
                {
                    InventoryDictionary processorInputs = _processor.GetInputs();
                    while (!needSelected)
                    {
                        int multiplier = 2;
                        foreach (KeyValuePair<ResourceType, float> need in processorInputs)
                        {
                            if (_agents[0].CheckInventoryFor(need.Key) < (need.Value * multiplier))
                            {
                                needSelected = true;
                                pickupType = need.Key;
                                pickupAmount = need.Value - _agents[0].CheckInventoryFor(need.Key);

                                nearestStore = _kingdomManager.NearestResourceStoreOfType(need.Key, _agents[0].transform.position, out distanceToNearestStore, true);
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
                            _agents[0].state = AgentState.STORING;
                    }
                    else
                    {
                        if (distanceToNearestStore <= _minPickupDistance)
                        {
                            float pickedUp = nearestStore.TakeResources(pickupType, pickupAmount);

                            float leftover = _agents[0].AddToInventory(pickupType, pickedUp);

                            if (leftover > 0)
                            {
                                nearestStore.AddResources(pickupType, leftover);
                            }
                        }
                        else
                        {
                            _agents[0].SetMovingTowards(nearestStore.transform.position, _minPickupDistance);
                        }
                    }
                }

                if (_agents[0].GetInventorySpace() <= 0 || !needSelected)
                {
                    _agents[0].state = AgentState.STORING;
                }
            }
            else if (_agents[0].state == AgentState.STORING)
            {
                float distanceToProcessor = (_agents[0].transform.position - _processor.transform.position).magnitude;

                if (distanceToProcessor <= _minProcessingDistance)
                {
                    foreach (KeyValuePair<ResourceType, float> need in _currentProcessorNeeds)
                    {
                        float fromInventory = _agents[0].RemoveFromInventory(need.Key);

                        float leftover = _processor.AddResources(need.Key, fromInventory);

                        if (leftover > 0)
                        {
                            _agents[0].AddToInventory(need.Key, leftover);
                        }
                    }

                    if (_agents[0].GetCurrentTotalInventory() > 0)
                    {
                        //Inventory has other items in it
                        _agents[0].state = AgentState.CLEAR_INVENTORY;
                    }
                    else
                    {
                        _agents[0].state = AgentState.WAITING;
                    }
                }
                else
                {
                    _agents[0].SetMovingTowards(_processor.transform.position, _minProcessingDistance);
                }
            }

        }

        if (state == GuildState.INACTIVE)
        {
            if (!_processor.HasNeeds())
            {
                state = GuildState.ACTIVE;
            }
            else
            {
                //TODO: determine when there are enough resources to reactivate
                bool enough = true;
                foreach (KeyValuePair<ResourceType, float> need in _currentProcessorNeeds)
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

        base.Update();
    }
}
