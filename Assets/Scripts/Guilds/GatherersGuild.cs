using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherersGuild : Guild
{
    public ResourceType resourceType = ResourceType.NONE;

    [SerializeField]
    protected float gatherSpeed = 10.0f;
    [SerializeField]
    protected float minGatherDistance = 1.0f;
    [SerializeField]
    protected float minStoreDistance = 1.0f;


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
                        agent.state = AgentState.GATHERING;
                    }
                    else
                    {
                        agent.state = AgentState.STORING;
                    }
                }

                if (agent.state == AgentState.GATHERING)
                {
                    Gatherable nearestGatherable = _naturalWorldManager.NearestGatherableOfType(resourceType, agent.transform.position);

                    if (nearestGatherable == null)
                    {
                        //No resource sources
                        state = GuildState.INACTIVE;
                        break;
                    }

                    if ((nearestGatherable.transform.position - agent.transform.position).magnitude <= minGatherDistance)
                    {
                        //near a source
                        float maxGathered = gatherSpeed * Time.deltaTime;

                        float gathered = nearestGatherable.HarvestResources(maxGathered);

                        float leftover = agent.AddToInventory(resourceType, gathered);

                        if (leftover > 0)
                        {
                            nearestGatherable.AddResources(leftover);
                        }
                    }
                    else
                    {
                        agent.SetMovingTowards(nearestGatherable.transform.position, minGatherDistance);
                    }

                    if (agent.GetInventorySpace() == 0)
                    {
                        agent.state = AgentState.STORING;
                    }
                }
                else if (agent.state == AgentState.STORING)
                {

                    SingleResourceStore nearestStore = _kingdomManager.NearestSingleResourceStoreOfType(resourceType, agent.transform.position);

                    if (nearestStore == null)
                    {
                        //No storage spots
                        state = GuildState.INACTIVE;
                        break;
                    }

                    if ((nearestStore.transform.position - agent.transform.position).magnitude <= minStoreDistance)
                    {
                        //near a store
                        float woodFromInventory = agent.RemoveFromInventory(resourceType);

                        float leftover = nearestStore.AddResources(woodFromInventory);

                        if (leftover > 0)
                        {
                            agent.AddToInventory(resourceType, leftover);
                        }
                    }
                    else
                    {
                        agent.SetMovingTowards(nearestStore.transform.position, minStoreDistance);
                    }

                    if (agent.CheckInventoryFor(resourceType) == 0)
                    {
                        agent.state = AgentState.GATHERING;
                    }
                }

            }
        }

        if (state == GuildState.INACTIVE)
        {
            if (_kingdomManager.FirstSingleResourceStoreOfType(resourceType) != null && _naturalWorldManager.FirstGatherableOfType(resourceType) != null)
            {
                state = GuildState.ACTIVE;
            }
        }
    }
}
