using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaturalWorldManager : MonoBehaviour
{
    [SerializeField] //Temporalily Serialized for testing
    private List<Gatherable> _gatherables;
    private List<SelfSpawningGatherable> _selfSpawningGatherables;

    public ContactFilter2D selfSpawningZoneFilter;

    void Awake()
    {
        _gatherables = new List<Gatherable>();
        _selfSpawningGatherables = new List<SelfSpawningGatherable>();
    }

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

    public void AddSelfSpawningGatherable(SelfSpawningGatherable gatherable)
    {
        _selfSpawningGatherables.Add(gatherable);
    }
    public void RemoveSelfSpawningGatherable(SelfSpawningGatherable gatherable)
    {
        _selfSpawningGatherables.Remove(gatherable);
    }

    public Gatherable NearestGatherableOfType(ResourceType type, Vector3 position)
    {
        float shortestDistance = float.MaxValue;
        Gatherable nearestGatherable = null;

        //TODO: optimise
        foreach (Gatherable g in _gatherables)
        {
            if (g.resourceType == type && g.GetCurrentResources() > 0.0f && g.state != GatherableState.NON_GATHERABLE && g.CanTakeMoreGatherers())
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
    public Gatherable NearestGatherableOfType(ResourceType type, Vector3 position, out float shortestDistance)
    {
        shortestDistance = float.MaxValue;
        Gatherable nearestGatherable = null;

        //TODO: optimise
        foreach (Gatherable g in _gatherables)
        {
            if (g.resourceType == type && g.GetCurrentResources() > 0.0f && g.state != GatherableState.NON_GATHERABLE && g.CanTakeMoreGatherers())
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
            if (g.resourceType == type && g.GetCurrentResources() > 0.0f && g.state == GatherableState.GATHERABLE_READY && g.CanTakeMoreGatherers())
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
    public Gatherable NearestReadyGatherableOfType(ResourceType type, Vector3 position, out float shortestDistance)
    {
        shortestDistance = float.MaxValue;
        Gatherable nearestGatherable = null;

        //TODO: optimise
        foreach (Gatherable g in _gatherables)
        {
            if (g.resourceType == type && g.GetCurrentResources() > 0.0f && g.state == GatherableState.GATHERABLE_READY && g.CanTakeMoreGatherers())
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
            if (g.resourceType == type && g.GetCurrentResources() > 0.0f && g.state != GatherableState.NON_GATHERABLE && g.CanTakeMoreGatherers())
            {
                return g;
            }
        }

        return null;
    }

    public bool CanSpawn(Vector3 position, float exclusionRadius)
    {
        List<RaycastHit2D> raycastHits = new List<RaycastHit2D>();
        if (Physics2D.Raycast(position, Vector2.zero, selfSpawningZoneFilter, raycastHits) <= 0)
        {
            return false;
        }
        foreach (SelfSpawningGatherable gatherable in _selfSpawningGatherables)
        {
            float distance = (gatherable.transform.position - position).magnitude;
            if (distance <= exclusionRadius)
            {
                return false;
            }
        }
        return true;
    }
}
