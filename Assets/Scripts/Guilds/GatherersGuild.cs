using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherersGuild : Guild
{
    public ResourceType resourceType = ResourceType.NONE;

    [SerializeField]
    protected float _gatherSpeed = 10.0f;
    [SerializeField]
    protected float _minGatherDistance = 0.2f;
    [SerializeField]
    protected float _minStoreDistance = 0.0f;

    [SerializeField]
    protected float _prioritiseReadyGatherablesWithin = 5.0f;

    [SerializeField]
    protected bool _onlyGatherFromReadyGatherables = false;

    protected string _priorityName;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        gameObject.tag = resourceType + "GatherersGuild";

        _priorityName = "gather" + resourceType;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    protected override void UpdateTargetAgentCount()
    {
        float priorityFactor = _kingdomManager.GetPriority(_priorityName) / _kingdomManager.GetTotalPriority();

        targetAgentCount = Mathf.CeilToInt((float)_kingdomManager.GetAgentCount() * priorityFactor);
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
                }
                else
                {
                    agent.state = AgentState.STORING;
                }
            }

            if (agent.state == AgentState.COLLECTING)
            {
                float distanceToNearestGatherable;
                Gatherable nearestGatherable;
                if (_onlyGatherFromReadyGatherables)
                {
                    nearestGatherable = _naturalWorldManager.NearestReadyGatherableOfType(resourceType, agent.transform.position, out distanceToNearestGatherable);
                }
                else
                {
                    nearestGatherable = _naturalWorldManager.NearestGatherableOfType(resourceType, agent.transform.position, out distanceToNearestGatherable);

                    _naturalWorldManager.NearestReadyGatherableOfType(resourceType, agent.transform.position, out float distanceToNearestReadyGatherable);

                    if (distanceToNearestReadyGatherable - distanceToNearestGatherable < _prioritiseReadyGatherablesWithin)
                    {
                        // If nearest ready gatherable is not too much further away
                        nearestGatherable = _naturalWorldManager.NearestReadyGatherableOfType(resourceType, agent.transform.position, out distanceToNearestGatherable);
                    }
                }

                if (nearestGatherable == null)
                {
                    //No resource sources
                    if (agent.GetInventorySpace() > 0)
                        state = GuildState.INACTIVE;
                }
                else
                {
                    if (distanceToNearestGatherable <= _minGatherDistance)
                    {
                        //near a source
                        float maxGathered = _gatherSpeed * Time.deltaTime + agent.GetResidualWork();
                        int maxGatheredInt = Mathf.FloorToInt(maxGathered);
                        agent.SetResidualWork(maxGathered - maxGatheredInt);

                        int gathered = nearestGatherable.HarvestResources(maxGatheredInt);

                        int leftover = agent.AddToInventory(resourceType, gathered);

                        if (leftover > 0)
                        {
                            nearestGatherable.AddResources(leftover);
                        }
                    }
                    else
                    {
                        agent.SetMovingTowards(nearestGatherable.transform.position, _minGatherDistance);
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
