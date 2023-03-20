using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*Task flows:
 COLLECTING -> DROP_OFF -> WAITING
 WORKING -> WAITING / null target
 UNWORKING -> WAITING
 PICK_UP -> CLEAR_INVENTORY
 */
public class ConstructionGuild : Guild
{
    protected List<Construction> _waitingForResourcesConstructions;
    protected List<Construction> _buildingConstructions;
    protected List<Construction> _deconstructingConstructions;
    protected List<Construction> _waitingForEmptyConstructions;

    // Start is called before the first frame update
    new void Start()
    {
        gameObject.tag = "ConstructionGuild";

        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        _waitingForResourcesConstructions = _kingdomManager.WaitingForResourcesConstructions();
        _buildingConstructions = _kingdomManager.BuildingConstructions();
        _deconstructingConstructions = _kingdomManager.DeconstructingConstructions();
        _waitingForEmptyConstructions = _kingdomManager.WaitingForEmptyConstructions();

        base.Update();
    }


    protected override void InitaliseGuildTaskValidity()
    {
        _guildTaskValidity = new Dictionary<AgentState, bool>();

        //Get resources from stores
        _guildTaskValidity.Add(AgentState.COLLECTING, true);

        //Add resources to constructions 
        //Not assigned from WAITING

        //Build constructions
        _guildTaskValidity.Add(AgentState.WORKING, true);

        //Deconstruct constructions
        _guildTaskValidity.Add(AgentState.UNWORKING, true);

        //Take resources from canceled constructions
        _guildTaskValidity.Add(AgentState.PICK_UP, true);

        //Store resources taken from canceled constructions
        //Not assigned from WAITING
    }
    protected override void SetPriorityName()
    {
        _priorityName = "construct";
    }


    protected override void CheckTasks()
    {
        /*Task flows:
         COLLECTING -> DROP_OFF -> WAITING
         WORKING -> WAITING / null target
         UNWORKING -> WAITING
         PICK_UP -> CLEAR_INVENTORY
         */

        // Check COLLECTING
        bool canCollect = false;

        foreach (Construction construction in _waitingForResourcesConstructions)
        {
            if (construction.CanTakeMoreAgents()) //returns true only when avalible resources to meet the needs that are more than AssignedAgents * AgentInventorySpace
            {
                canCollect = true;
                break;
            }
        }
        _guildTaskValidity[AgentState.COLLECTING] = canCollect;

        // Check WORKING
        bool canWork = false;
        foreach (Construction construction in _buildingConstructions)
        {
            if (construction.CanTakeMoreAgents())
            {
                canWork = true;
                break;
            }
        }

        _guildTaskValidity[AgentState.WORKING] = canWork;

        // Check UNWORKING
        bool canUnwork = false;

        foreach (Construction construction in _deconstructingConstructions)
        {
            if (construction.CanTakeMoreAgents())
            {
                canUnwork = true;
                break;
            }
        }

        _guildTaskValidity[AgentState.UNWORKING] = canUnwork;


        // Check PICK_UP
        bool canPickUp = false;

        foreach (Construction construction in _waitingForEmptyConstructions)
        {
            if (construction.CanTakeMoreAgents()) //returns true only when storeable resources is more than AssignedAgents * AgentInventorySpace
            {
                canPickUp = true;
                break;
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
        /*Task flows:
         COLLECTING -> DROP_OFF -> WAITING
         WORKING -> WAITING / null target
         UNWORKING -> WAITING
         PICK_UP -> CLEAR_INVENTORY
         */
        foreach (Agent agent in _agents)
        {
            if (agent.state == AgentState.WAITING)
            {
                Construction targetConstruction = agent.targetBuilding as Construction;
                if (targetConstruction != null)
                {
                    UpdateAgentStateForConstruction(agent, targetConstruction);
                }
                else
                {
                    AssignWaitingAgent(agent);
                }
            }

            if (agent.state == AgentState.COLLECTING)
            {
                Construction targetConstruction = CheckAndUpdateAssignedBuilding(agent, ConstructionState.WAITING_FOR_RESOURCES, _waitingForResourcesConstructions);
                if (targetConstruction == null)
                    continue;

                InventoryDictionary constructionNeedsExcludingInventories = targetConstruction.GetFillableNeeds(true);

                if (!constructionNeedsExcludingInventories.ContainsKey(agent.targetResource))
                {
                    if (constructionNeedsExcludingInventories.Count > 0)
                    {
                        ResourceType randomNeed = (new List<ResourceType>(constructionNeedsExcludingInventories.Keys))[Random.Range(0, constructionNeedsExcludingInventories.Count)];
                        agent.SetTargetResource(randomNeed);
                    }
                    else
                    {
                        agent.state = AgentState.DROP_OFF;
                        continue;
                    }
                }

                ResourceStore nearestStore = _kingdomManager.NearestResourceStoreOfType(agent.targetResource, agent.transform.position, true);
                int pickupAmount = constructionNeedsExcludingInventories[agent.targetResource];

                if (nearestStore != null)
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
                Construction targetConstruction = agent.targetBuilding as Construction;
                if (targetConstruction == null)
                    Debug.LogError("Can't find targetConstruction for " + agent.name + " in " + agent.state + " state.");


                float distanceToConstruction = (agent.transform.position - targetConstruction.transform.position).magnitude;

                if (distanceToConstruction <= _minInteractionDistance)
                {
                    InventoryDictionary constructionNeeds = targetConstruction.GetFillableNeeds();

                    foreach (KeyValuePair<ResourceType, int> need in constructionNeeds)
                    {
                        int fromInventory = agent.RemoveFromInventory(need.Key);

                        int leftover = targetConstruction.AddResources(need.Key, fromInventory);

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
                    agent.SetMovingTowards(targetConstruction.transform.position, _minInteractionDistance);
                }
            }
            else if (agent.state == AgentState.WORKING)
            {
                Construction targetConstruction = CheckAndUpdateAssignedBuilding(agent, ConstructionState.BUILDING, _buildingConstructions);
                if (targetConstruction == null)
                    continue;


                float distanceToConstruction = (agent.transform.position - targetConstruction.transform.position).magnitude;

                if (distanceToConstruction <= _minInteractionDistance)
                {
                    if (!targetConstruction.Build(Time.deltaTime))
                    {
                        agent.state = AgentState.WAITING;
                    }
                }
                else
                {
                    agent.SetMovingTowards(targetConstruction.transform.position, _minInteractionDistance);
                }
            }
            else if (agent.state == AgentState.UNWORKING)
            {
                Construction targetConstruction = CheckAndUpdateAssignedBuilding(agent, ConstructionState.DECONSTRUCTING, _deconstructingConstructions);
                if (targetConstruction == null)
                    continue;


                float distanceToConstruction = (agent.transform.position - targetConstruction.transform.position).magnitude;

                if (distanceToConstruction <= _minInteractionDistance)
                {
                    if (!targetConstruction.Deconstruct(Time.deltaTime))
                    {
                        agent.state = AgentState.WAITING;
                    }
                }
                else
                {
                    agent.SetMovingTowards(targetConstruction.transform.position, _minInteractionDistance);
                }
            }
            else if (agent.state == AgentState.PICK_UP)
            {
                Construction targetConstruction = CheckAndUpdateAssignedBuilding(agent, ConstructionState.WAITING_FOR_EMPTY, _waitingForEmptyConstructions);
                if (targetConstruction == null)
                    continue;

                float distanceToConstruction = (agent.transform.position - targetConstruction.transform.position).magnitude;

                if (distanceToConstruction <= _minInteractionDistance)
                {
                    foreach (KeyValuePair<ResourceType, int> resource in targetConstruction.GetCurrentResources())
                    {
                        int maxToTake = agent.GetInventorySpace();

                        int amountTaken = targetConstruction.TakeResources(resource.Key, maxToTake);

                        agent.AddToInventory(resource.Key, amountTaken);
                    }

                    agent.state = AgentState.CLEAR_INVENTORY;
                }
                else
                {
                    agent.SetMovingTowards(targetConstruction.transform.position, _minInteractionDistance);
                }
            }

        }
    }
    protected override void InactiveUpdate()
    {
        foreach (Agent agent in _agents)
        {
            Construction targetConstruction = agent.targetBuilding as Construction;
            if (targetConstruction != null)
            {
                if (!targetConstruction.HasTooManyAgents())
                {
                    UpdateAgentStateForConstruction(agent, targetConstruction);
                }
                else
                {
                    //clear assignment 
                    agent.SetTargetBuilding(null);
                }
            }
        }
    }


    //returns target construction
    protected Construction CheckAndUpdateAssignedBuilding(Agent agent, ConstructionState expectedStateInAssignedBuilding, List<Construction> constructionsWithExpectedState)
    {
        #region Check if assigned building valid for this state, change state if assigned building valid for some task
        Construction targetConstruction = agent.targetBuilding as Construction;
        if (targetConstruction != null)
        {
            //if target invalid
            if (targetConstruction.state != expectedStateInAssignedBuilding)
            {
                //if target valid at all
                if (!targetConstruction.HasTooManyAgents())
                {
                    //reassign to valid task for target
                    UpdateAgentStateForConstruction(agent, targetConstruction);
                    return null;
                }
                else
                {
                    //clear assignment 
                    agent.SetTargetBuilding(null);
                }
            }
        }
        #endregion
        #region Assign to building if not already
        else
        {
            foreach (Construction construction in constructionsWithExpectedState)
            {
                if (construction.CanTakeMoreAgents())
                {
                    agent.SetTargetBuilding(construction);
                    targetConstruction = construction;
                }
            }
        }
        #endregion
        if (targetConstruction == null)
        {
            Debug.Log("Target construction not assigned while in " + agent.state + " state.");
            agent.state = AgentState.WAITING;
        }

        return targetConstruction;
    }
    public void UpdateAgentStateForConstruction(Agent agent, Construction construction)
    {
        if (!construction.HasAgent(agent))
        {
            Debug.LogError("Called \"UpdateAgentStateForConstruction\" passing an agent not assigned to passed construction.");
            return;
        }
        /*Task flows:
         COLLECTING -> DROP_OFF -> WAITING
         WORKING -> WAITING
         UNWORKING -> WAITING
         PICK_UP -> STORING -> WAITING
         */
        if (construction.state == ConstructionState.WAITING_FOR_RESOURCES)
        {
            agent.state = AgentState.COLLECTING;
        }
        else if (construction.state == ConstructionState.BUILDING)
        {
            agent.state = AgentState.WORKING;
        }
        else if (construction.state == ConstructionState.DECONSTRUCTING)
        {
            agent.state = AgentState.UNWORKING;
        }
        else //if (construction.state == COnstructionState.WAITING_FOR_EMPTY
        {
            agent.state = AgentState.PICK_UP;
        }
    }
}
