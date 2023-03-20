using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherersGuild : Guild
{
    public ResourceType resourceType = ResourceType.NONE;

    [SerializeField]
    protected float _gatherSpeed = 10.0f;

    [SerializeField]
    protected float _prioritiseReadyGatherablesWithin = 5.0f;

    [SerializeField]
    protected bool _onlyGatherFromReadyGatherables = false;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        gameObject.tag = resourceType + "GatherersGuild";
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }


    protected override void SetPriorityName()
    {
        _priorityName = "gather" + resourceType;
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
                if (agent.GetInventorySpace() > 0)
                {
                    agent.state = AgentState.COLLECTING;
                    agent.SetTargetResource(resourceType);
                }
                else
                {
                    agent.state = AgentState.STORING;
                }
            }

            if (agent.state == AgentState.COLLECTING)
            {
                float distanceToGatherable;
                if (agent.targetGatherable == null)
                {
                    if (_onlyGatherFromReadyGatherables)
                    {
                        agent.SetTargetGatherable(_naturalWorldManager.NearestReadyGatherableOfType(resourceType, agent.transform.position, out distanceToGatherable));
                    }
                    else
                    {
                        agent.SetTargetGatherable(_naturalWorldManager.NearestGatherableOfType(resourceType, agent.transform.position, out distanceToGatherable));

                        Gatherable nearestReadyGatherable = _naturalWorldManager.NearestReadyGatherableOfType(resourceType, agent.transform.position, out float distanceToNearestReadyGatherable);

                        if (distanceToNearestReadyGatherable - distanceToGatherable < _prioritiseReadyGatherablesWithin)
                        {
                            // If nearest ready gatherable is not too much further away
                            agent.SetTargetGatherable(nearestReadyGatherable);
                            distanceToGatherable = distanceToNearestReadyGatherable;
                        }
                    }
                }
                else
                {
                    distanceToGatherable = (agent.targetGatherable.transform.position - agent.transform.position).magnitude;
                }

                if (agent.targetGatherable == null)
                {
                    //No resource sources
                    if (agent.GetInventorySpace() > 0)
                        state = GuildState.INACTIVE;
                }
                else
                {
                    if (distanceToGatherable <= _minInteractionDistance)
                    {
                        //near a source
                        float maxGathered = _gatherSpeed * Time.deltaTime + agent.GetResidualWork();
                        int maxGatheredInt = Mathf.FloorToInt(maxGathered);
                        agent.SetResidualWork(maxGathered - maxGatheredInt);

                        int gathered = agent.targetGatherable.HarvestResources(maxGatheredInt);

                        int leftover = agent.AddToInventory(resourceType, gathered);

                        if (leftover > 0)
                        {
                            agent.targetGatherable.AddResources(leftover);
                        }
                    }
                    else
                    {
                        agent.SetMovingTowards(agent.targetGatherable.transform.position, _minInteractionDistance);
                    }
                }

                if (agent.GetInventorySpace() <= 0)
                {
                    agent.state = AgentState.STORING;
                }
            }
            else if (agent.state == AgentState.STORING)
            {

                ResourceStore nearestStore = _kingdomManager.NearestResourceStoreOfType(resourceType, agent.transform.position);

                if (nearestStore == null)
                {
                    //No storage spots
                    state = GuildState.INACTIVE;
                    break;
                }

                if ((nearestStore.transform.position - agent.transform.position).magnitude <= _minStoreDistance)
                {
                    //near a store
                    int fromInventory = agent.RemoveFromInventory(resourceType);

                    int leftover = nearestStore.AddResources(resourceType, fromInventory);

                    if (leftover > 0)
                    {
                        agent.AddToInventory(resourceType, leftover);
                    }
                }
                else
                {
                    agent.SetMovingTowards(nearestStore.transform.position, _minStoreDistance);
                }

                if (agent.CheckInventoryFor(resourceType) <= 0)
                {
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
            }

        }
    }
    protected override void InactiveUpdate()
    {

        if (_kingdomManager.FirstResourceStoreOfType(resourceType) != null && _naturalWorldManager.FirstGatherableOfType(resourceType) != null)
        {
            state = GuildState.ACTIVE;
        }
    }
}
