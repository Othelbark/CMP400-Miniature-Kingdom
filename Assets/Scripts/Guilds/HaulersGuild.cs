using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// COLLECTING from priority 0 
// DROP_OFF to priority 1

// PICK_UP from priority 1
// STORING to priority 2

public class HaulersGuild : Guild
{

    public ResourceType[] prioritisedHaulingList;

    protected List<ResourceType> _forHauling0to1;
    protected List<ResourceType> _forHauling1to2;

    // Start is called before the first frame update
    new void Start()
    {
        gameObject.tag = "HaulersGuild";

        _forHauling0to1 = new List<ResourceType>();
        _forHauling1to2 = new List<ResourceType>();

        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    protected override void InitaliseGuildTaskValidity()
    {
        _guildTaskValidity = new Dictionary<AgentState, bool>();

        _guildTaskValidity.Add(AgentState.PICK_UP, true);
        _guildTaskValidity.Add(AgentState.COLLECTING, true);
    }

    protected override void SetPriorityName()
    {
        _priorityName = "haul";
    }


    protected override void CheckTasks()
    {
        _forHauling0to1.Clear();
        _forHauling1to2.Clear();

        // Check COLLECTING
        bool canCollect = false;

        //Check for valid transfers from priotity 0 to 1

        foreach (ResourceType type in prioritisedHaulingList)
        {
            if (_kingdomManager.GetTotalSpaceFor(type, true, 1) > 0)
            {
                _forHauling0to1.Add(type);
            }

            if (_kingdomManager.FirstResourceStoreOfType(type, true, 0) != null && _kingdomManager.GetTotalSpaceFor(type, true, 1) - TotalHeldInAssignedAgents(type) > 0)
            {
                canCollect = true;
            }
        }

        _guildTaskValidity[AgentState.COLLECTING] = canCollect;

        // Check PICK_UP
        bool canPickUp = false;

        //Check for valid transfers from priotity 1 to 2

        foreach (ResourceType type in prioritisedHaulingList)
        {
            if (_kingdomManager.GetTotalSpaceFor(type, true, 2) > 0)
            {
                _forHauling1to2.Add(type);
            }

            if (_kingdomManager.FirstResourceStoreOfType(type, true, 1) != null && _kingdomManager.GetTotalSpaceFor(type, true, 2) - TotalHeldInAssignedAgents(type) > 0)
            {
                canPickUp = true;
            }
        }

        _guildTaskValidity[AgentState.PICK_UP] = canPickUp;
    }


    protected override void UpdateTargetAgentCount()
    {
        base.UpdateTargetAgentCount();
    }
    protected override void ActiveUpdate()
    {
        foreach (Agent agent in _agents)
        {
            if (agent.state == AgentState.WAITING)
            {
                AssignWaitingAgent(agent);
            }

            if (agent.state == AgentState.COLLECTING)
            {

                if (!_forHauling0to1.Contains(agent.targetResource) || 
                    _kingdomManager.GetTotalSpaceFor(agent.targetResource, true, 1) - TotalHeldInAssignedAgents(agent.targetResource) <= 0 || 
                    _kingdomManager.FirstResourceStoreOfType(agent.targetResource, true, 0) == null)
                {
                    if (_forHauling0to1.Count > 0)
                    {
                        List<ResourceType> needs = new List<ResourceType>();
                        foreach (ResourceType type in _forHauling0to1)
                        {
                            if (_kingdomManager.GetTotalSpaceFor(type, true, 1) - TotalHeldInAssignedAgents(type) > 0 && _kingdomManager.FirstResourceStoreOfType(type, true, 0) != null)
                            {
                                needs.Add(type);
                            }
                        }

                        ResourceType need = ResourceType.NONE;
                        if (needs.Count > 0)
                        {
                            need = needs[Random.Range(0, needs.Count - 1)];
                        }


                        if (need != ResourceType.NONE)
                        {
                            agent.SetTargetResource(need);
                        }
                        else
                        {
                            agent.state = AgentState.DROP_OFF;
                            continue;
                        }
                    }
                    else
                    {
                        agent.state = AgentState.DROP_OFF;
                        continue;
                    }
                }

                ResourceStore nearestStore = _kingdomManager.NearestResourceStoreOfType(agent.targetResource, agent.transform.position, true, 0);
                int pickupAmount = _kingdomManager.GetTotalSpaceFor(agent.targetResource, true, 1) - TotalHeldInAssignedAgents(agent.targetResource);

                if (nearestStore != null && pickupAmount > 0)
                {
                    CollectFromStore(agent, nearestStore, agent.targetResource, pickupAmount);
                }

                if (agent.GetInventorySpace() <= 0 || nearestStore == null)
                {
                    agent.state = AgentState.DROP_OFF;
                }
            }
            else if (agent.state == AgentState.DROP_OFF)
            {
                ResourceType typeToStore = ResourceType.NONE;

                foreach (ResourceType type in _forHauling0to1)
                {
                    if (agent.CheckInventoryFor(type) > 0)
                    {
                        typeToStore = type;
                        break;
                    }
                }

                if(_forHauling1to2.Contains(typeToStore))
                {
                    agent.state = AgentState.STORING;
                    continue;
                }


                if (typeToStore != ResourceType.NONE)
                {
                    float distanceToNearestStore;
                    ResourceStore nearestStore = _kingdomManager.NearestResourceStoreOfType(typeToStore, agent.transform.position, out distanceToNearestStore, false, 1);

                    if (nearestStore == null)
                    {
                        agent.state = AgentState.CLEAR_INVENTORY;
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
                    if (agent.GetCurrentTotalInventory() > 0)
                    {
                        agent.state = AgentState.CLEAR_INVENTORY;
                    }
                    else
                    {
                        agent.state = AgentState.WAITING;
                    }
                }
            }
            else if (agent.state == AgentState.PICK_UP)
            {

                if (!_forHauling1to2.Contains(agent.targetResource) ||
                    _kingdomManager.GetTotalSpaceFor(agent.targetResource, true, 2) - TotalHeldInAssignedAgents(agent.targetResource) <= 0 ||
                    _kingdomManager.FirstResourceStoreOfType(agent.targetResource, true, 1) == null)
                {
                    if (_forHauling1to2.Count > 0)
                    {
                        List<ResourceType> needs = new List<ResourceType>();
                        foreach (ResourceType type in _forHauling1to2)
                        {
                            if (_kingdomManager.GetTotalSpaceFor(type, true, 2) - TotalHeldInAssignedAgents(type) > 0 && _kingdomManager.FirstResourceStoreOfType(type, true, 1) != null)
                            {
                                needs.Add(type);
                            }
                        }

                        ResourceType need = ResourceType.NONE;
                        if (needs.Count > 0)
                        {
                            need = needs[Random.Range(0, needs.Count - 1)];
                        }


                        if (need != ResourceType.NONE)
                        {
                            agent.SetTargetResource(need);
                        }
                        else
                        {
                            agent.state = AgentState.STORING;
                            continue;
                        }
                    }
                    else
                    {
                        agent.state = AgentState.STORING;
                        continue;
                    }
                }

                ResourceStore nearestStore = _kingdomManager.NearestResourceStoreOfType(agent.targetResource, agent.transform.position, true, 1);
                int pickupAmount = _kingdomManager.GetTotalSpaceFor(agent.targetResource, true, 2) - TotalHeldInAssignedAgents(agent.targetResource);

                if (nearestStore != null && pickupAmount > 0)
                {
                    CollectFromStore(agent, nearestStore, agent.targetResource, pickupAmount);
                }

                if (agent.GetInventorySpace() <= 0 || nearestStore == null)
                {
                    agent.state = AgentState.STORING;
                }
            }
            else if (agent.state == AgentState.STORING)
            {

                ResourceType typeToStore = ResourceType.NONE;

                foreach (ResourceType type in _forHauling1to2)
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
                    ResourceStore nearestStore = _kingdomManager.NearestResourceStoreOfType(typeToStore, agent.transform.position, out distanceToNearestStore, false, 2);

                    if (nearestStore == null)
                    {
                        agent.state = AgentState.CLEAR_INVENTORY;
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
                    if (agent.GetCurrentTotalInventory() > 0)
                    {
                        agent.state = AgentState.CLEAR_INVENTORY;
                    }
                    else
                    {
                        agent.state = AgentState.WAITING;
                    }
                }
            }

        }
    }
    protected override void InactiveUpdate()
    {
    }


    protected int TotalHeldInAssignedAgents(ResourceType type)
    {
        int total = 0;

        foreach (Agent agent in _agents)
        {
            total += agent.CheckInventoryFor(type);
        }

        return total;
    }
}
