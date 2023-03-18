using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*Task flows:
 COLLECTING -> DROP_OFF -> WAITING
 WORKING -> WAITING
 PICK_UP -> STORING -> WAITING
 */
public class ConstructionGuild : Guild
{
    protected List<Construction> _waitingConstructions;
    protected List<Construction> _buildingConstructions;
    protected List<Construction> _deconstructingConstructions;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        _waitingConstructions = _kingdomManager.WaitingConstructions();
        _buildingConstructions = _kingdomManager.BuildingConstructions();
        _deconstructingConstructions = _kingdomManager.DeconstructingConstructions();

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
         PICK_UP -> STORING -> WAITING
         */

        // Check COLLECTING
        bool canCollect = false;

        foreach (Construction construction in _waitingConstructions)
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

        // Check PICK_UP
        bool canPickUp = false;

        foreach (Construction construction in _deconstructingConstructions)
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
                if (agent.targetBuilding == null)
                {
                    foreach (Construction construction in _waitingConstructions)
                    {
                        if (construction.CanTakeMoreAgents())
                        {
                            agent.SetTargetBuilding(construction);
                        }
                    }
                }

            }
            else if (agent.state == AgentState.DROP_OFF)
            {
            }
            else if (agent.state == AgentState.WORKING)
            {
            }
            else if (agent.state == AgentState.PICK_UP)
            {
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
