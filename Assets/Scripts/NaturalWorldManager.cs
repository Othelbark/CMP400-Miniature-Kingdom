using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaturalWorldManager : MonoBehaviour
{
    [SerializeField] //Temporalily Serialized for testing
    private List<Gatherable> _gatherables;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddGatherable(Gatherable gatherable)
    {
        _gatherables.Add(gatherable);
    }

    public void RemoveGatherable(Gatherable gatherable)
    {
        _gatherables.Remove(gatherable);
    }

    public Gatherable NearestGatherableOfType(ResourceType type, Vector3 position)
    {
        float shortestDistance = float.MaxValue;
        Gatherable nearestGatherable = null;

        //TODO: optimise
        foreach (Gatherable g in _gatherables)
        {
            if (g.resourceType == type && g.GetCurrentResources() > 0.0f && g.state != GatherableState.NON_GATHERABLE)
            {
                float distance = (g.gameObject.transform.position - position).magnitude;
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestGatherable = g;
                }
            }
        }

        return nearestGatherable;
    }
    public Gatherable NearestReadyGatherableOfType(ResourceType type, Vector3 position)
    {
        float shortestDistance = float.MaxValue;
        Gatherable nearestGatherable = null;

        //TODO: optimise
        foreach (Gatherable g in _gatherables)
        {
            if (g.resourceType == type && g.GetCurrentResources() > 0.0f && g.state == GatherableState.GATHERABLE_READY)
            {
                float distance = (g.gameObject.transform.position - position).magnitude;
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestGatherable = g;
                }
            }
        }

        return nearestGatherable;
    }

    public Gatherable FirstGatherableOfType(ResourceType type)
    {
        foreach (Gatherable g in _gatherables)
        {
            if (g.resourceType == type && g.GetCurrentResources() > 0.0f && g.state != GatherableState.NON_GATHERABLE)
            {
                return g;
            }
        }

        return null;
    }
}
