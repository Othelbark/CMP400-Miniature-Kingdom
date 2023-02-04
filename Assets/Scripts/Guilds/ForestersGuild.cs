using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestersGuild : Guild
{
    public float woodGatherSpeed = 10.0f;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start(); 
    }

    // Update is called once per frame
    new void Update()
    {
        if (state == GuildState.ACTIVE)
        {
            foreach (Agent agent in _agents)
            {
                if (agent.state == AgentState.WAITING)
                {
                    if (agent.GetInventorySpace() > 0)
                    {
                        agent.state = AgentState.WOODCUTTING;
                    }
                    else
                    {
                        agent.state = AgentState.STORING;
                    }
                }

                if (agent.state == AgentState.WOODCUTTING)
                {
                    Gatherable nearestTree = _naturalWorldManager.NearestGatherableOfType(ResourceType.WOOD, agent.transform.position);

                    if (nearestTree == null)
                    {
                        //No wood sources
                        state = GuildState.INACTIVE;
                        break;
                    }

                    if ((nearestTree.transform.position - agent.transform.position).magnitude <= 1.0f)
                    {
                        //near a tree
                        float maxWoodGathered = woodGatherSpeed * Time.deltaTime;

                        float woodFromTree = nearestTree.HarvestResources(maxWoodGathered);

                        float leftover = agent.AddToInventory(ResourceType.WOOD, woodFromTree);

                        if (leftover > 0)
                        {
                            nearestTree.AddResources(leftover);
                        }
                    }
                    else
                    {
                        agent.SetMovingTowards(nearestTree.transform.position);
                    }

                    if (agent.GetInventorySpace() == 0)
                    {
                        agent.state = AgentState.STORING;
                    }
                }
                else if (agent.state == AgentState.STORING)
                {

                    SingleResourceStore nearestStore = _kingdomManager.NearestSingleResourceStoreOfType(ResourceType.WOOD, agent.transform.position);

                    if (nearestStore == null)
                    {
                        //No storage spots
                        state = GuildState.INACTIVE;
                        break;
                    }

                    if ((nearestStore.transform.position - agent.transform.position).magnitude <= 1.0f)
                    {
                        //near a store
                        float woodFromInventory = agent.RemoveFromInventory(ResourceType.WOOD);

                        float leftover = nearestStore.AddResources(woodFromInventory);

                        if (leftover > 0)
                        {
                            agent.AddToInventory(ResourceType.WOOD, leftover);
                        }
                    }
                    else
                    {
                        agent.SetMovingTowards(nearestStore.transform.position);
                    }

                    if (agent.CheckInventoryFor(ResourceType.WOOD) == 0)
                    {
                        agent.state = AgentState.WOODCUTTING;
                    }
                }

            }
        }

        if (state == GuildState.INACTIVE)
        {
            if (_kingdomManager.FirstSingleResourceStoreOfType(ResourceType.WOOD) != null && _naturalWorldManager.FirstGatherableOfType(ResourceType.WOOD) != null)
            {
                state = GuildState.ACTIVE;
            }
        }
    }
}
