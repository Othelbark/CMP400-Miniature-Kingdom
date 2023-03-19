using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*Task flows:
 COLLECTING -> DROP_OFF -> WAITING
 WORKING -> WAITING
 UNWORKING -> WAITING
 PICK_UP -> STORING -> WAITING
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


    protected override void CheckTasks()
    {
        /*Task flows:
         COLLECTING -> DROP_OFF -> WAITING
         WORKING -> WAITING
         UNWORKING -> WAITING
         PICK_UP -> STORING -> WAITING
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

    }
    protected override void ActiveUpdate()
    {
        /*Task flows:
         COLLECTING -> DROP_OFF -> WAITING
         WORKING -> WAITING
         UNWORKING -> WAITING
         PICK_UP -> STORING -> WAITING
         */
        foreach (Agent agent in _agents)
        {
            if (agent.state == AgentState.WAITING)
            {
                AssignWaitingAgent(agent);
            }

            if (agent.state == AgentState.COLLECTING)
            {
                if (CheckAndUpdateAssignedBuilding(agent, ConstructionState.WAITING_FOR_RESOURCES, _waitingForResourcesConstructions) == false)
                    continue;




            }
            else if (agent.state == AgentState.DROP_OFF)
            {
            }
            else if (agent.state == AgentState.WORKING)
            {
                if (CheckAndUpdateAssignedBuilding(agent, ConstructionState.BUILDING, _buildingConstructions) == false)
                    continue;
            }
            else if (agent.state == AgentState.UNWORKING)
            {
                if (CheckAndUpdateAssignedBuilding(agent, ConstructionState.DECONSTRUCTING, _deconstructingConstructions) == false)
                    continue;

            }
            else if (agent.state == AgentState.PICK_UP)
            {
                if (CheckAndUpdateAssignedBuilding(agent, ConstructionState.WAITING_FOR_EMPTY, _waitingForEmptyConstructions) == false)
                    continue;
            }
            else if (agent.state == AgentState.STORING)
            {
            }

        }
    }
    protected override void InactiveUpdate()
    {

    }


    //returns false when agent state changed
    protected bool CheckAndUpdateAssignedBuilding(Agent agent, ConstructionState expectedStateInAssignedBuilding, List<Construction> constructionsWithExpectedState)
    {

        #region Check if assigned building valid for this state, change state if assigned building valid for some task
        Construction targetConstruction = agent.targetBuilding as Construction;
        if (targetConstruction != null)
        {
            //if target invalid
            if (targetConstruction.state != expectedStateInAssignedBuilding)
            {
                //if target valid at all
                if (targetConstruction.CanTakeMoreAgents())
                {
                    //reassign to valid task for target
                    UpdateAgentStateForConstruction(agent, targetConstruction);
                    return false;
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
            Debug.LogError("Target construction not assigned while in " + agent.state + " state.");

        return true;
    }
    protected void UpdateAgentStateForConstruction(Agent agent, Construction construction)
    {
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
