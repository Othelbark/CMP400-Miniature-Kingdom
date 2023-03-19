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
        foreach (Agent agent in _agents)
        {
            if (agent.state == AgentState.WAITING)
            {
                AssignWaitingAgent(agent);
            }

            if (agent.state == AgentState.COLLECTING)
            {
                #region Assign to building if not already
                if (agent.targetBuilding == null)
                {
                    foreach (Construction construction in _waitingForResourcesConstructions)
                    {
                        if (construction.CanTakeMoreAgents())
                        {
                            agent.SetTargetBuilding(construction);
                        }
                    }
                }
                #endregion


            }
            else if (agent.state == AgentState.DROP_OFF)
            {
            }
            else if (agent.state == AgentState.WORKING)
            {
                #region Assign to building if not already
                if (agent.targetBuilding == null)
                {
                    foreach (Construction construction in _buildingConstructions)
                    {
                        if (construction.CanTakeMoreAgents())
                        {
                            agent.SetTargetBuilding(construction);
                        }
                    }
                }
                #endregion
            }
            else if (agent.state == AgentState.UNWORKING)
            {
                #region Assign to building if not already
                if (agent.targetBuilding == null)
                {
                    foreach (Construction construction in _deconstructingConstructions)
                    {
                        if (construction.CanTakeMoreAgents())
                        {
                            agent.SetTargetBuilding(construction);
                        }
                    }
                }
                #endregion

            }
            else if (agent.state == AgentState.PICK_UP)
            {
                #region Assign to building if not already
                if (agent.targetBuilding == null)
                {
                    foreach (Construction construction in _waitingForEmptyConstructions)
                    {
                        if (construction.CanTakeMoreAgents())
                        {
                            agent.SetTargetBuilding(construction);
                        }
                    }
                }
                #endregion
            }
            else if (agent.state == AgentState.STORING)
            {
            }

        }
    }
    protected override void InactiveUpdate()
    {

    }
}
