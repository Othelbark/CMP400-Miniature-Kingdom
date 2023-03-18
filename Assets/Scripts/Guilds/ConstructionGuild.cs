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
        base.Update();
    }

    protected override void InitaliseGuildTaskValidity()
    {
        _guildTaskValidity = new Dictionary<AgentState, bool>();

        //Get resources from stores
        _guildTaskValidity.Add(AgentState.COLLECTING, true);
        //Add resources to constructions
        _guildTaskValidity.Add(AgentState.DROP_OFF, true);
        //Build constructions
        _guildTaskValidity.Add(AgentState.WORKING, true);
        //Take resources from canceled constructions
        _guildTaskValidity.Add(AgentState.PICK_UP, true);
        //Store resources taken from canceled constructions
        _guildTaskValidity.Add(AgentState.STORING, true);
    }

    protected override void UpdateTargetAgentCount()
    {

    }
    protected override void CheckTasksAndActivity()
    {
        /*Task flows:
         COLLECTING -> DROP_OFF -> WAITING
         WORKING -> WAITING
         PICK_UP -> STORING -> WAITING
         */

        _waitingConstructions = _kingdomManager.WaitingConstructions();
        _buildingConstructions = _kingdomManager.BuildingConstructions();
        _deconstructingConstructions = _kingdomManager.DeconstructingConstructions();

        // Check DROP_OFF
        bool canDropOff = true;
        if (_waitingConstructions.Count <= 0)
        {
            canDropOff = false;
        }
        _guildTaskValidity[AgentState.DROP_OFF] = canDropOff;

        //Check COLLECTING
        bool canCollect = true;
        if (!canDropOff)
        {
            canCollect = false;
        }
        _guildTaskValidity[AgentState.COLLECTING] = canCollect;

    }
    protected override void ActiveUpdate()
    {
        foreach (Agent agent in _agents)
        {
            if (agent.state == AgentState.WAITING)
            {
            }

            if (agent.state == AgentState.COLLECTING)
            {
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
