using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Gatherable : MonoBehaviour
{
    [HideInInspector]
    public GatherableState state = GatherableState.GATHERABLE_READY;
    public ResourceType resourceType = ResourceType.NONE;

    [SerializeField]
    protected int _currentResources = 0;
    [SerializeField]
    protected bool _infiniteResorces = false;

    [SerializeField]
    protected List<Agent> gatherers;
    [SerializeField]
    protected int _maxGatherers = 1;

    // Reference to the Natural World Manager
    protected NaturalWorldManager _naturalWorldManager;

    protected bool _spent = false;

    // Start is called before the first frame update
    public void Start()
    {
        try
        {
            _naturalWorldManager = GameObject.FindGameObjectWithTag("NaturalWorldManager").GetComponent<NaturalWorldManager>();
        }
        catch
        {
            Debug.LogError("Can't find natural world manager.");
        }

        gatherers = new List<Agent>();

        _naturalWorldManager.AddGatherable(this);

        state = GatherableState.GATHERABLE_READY;
    }

    // Update is called once per frame
    public void Update()
    {
        if (_spent)
        {
            Spent();
        }
    }

    public bool AddGatherer(Agent gatherer, bool forceAdd = false)
    {
        if (gatherers.Count < _maxGatherers)
        {
            gatherers.Add(gatherer);
            return true;
        }
        if (forceAdd)
        {
            gatherers[0].SetTargetGatherable(null);
            gatherers.Add(gatherer);
            return true;
        }
        return false;
    }
    public void RemoveGatherer(Agent gatherer)
    {
        gatherers.Remove(gatherer);
    }
    public bool CanTakeMoreGatherers()
    {
        if (gatherers.Count < _maxGatherers)
        {
            return true;
        }
        return false;
    }

    public int GetCurrentResources()
    {
        if (_infiniteResorces)
        {
            return int.MaxValue;
        }
        return _currentResources;
    }

    public void SetCurrentResources(int r)
    {
        _currentResources = r;
    }

    public void AddResources(int r)
    {
        _currentResources += r;
    }

    //Returns amount actually harvested
    public virtual int HarvestResources(int r)
    {
        if (state == GatherableState.NON_GATHERABLE)
        {
            return 0;
        }

        if (_infiniteResorces)
        {
            return r;
        }

        if (_currentResources > r)
        {
            _currentResources -= r;

            CheckSpent();
            return r;
        }
        else
        {
            int leftoverResources = _currentResources;
            _currentResources = 0;

            CheckSpent();
            return leftoverResources;
        }

    }

    protected virtual void CheckSpent()
    {
        if (_currentResources <= 0 && !_infiniteResorces)
        {
            _spent = true;
        }
    }

    protected virtual void Spent()
    {
        foreach (Agent gatherer in gatherers)
        {
            gatherer.ClearTargetGatherable();
        }

        _naturalWorldManager.RemoveGatherable(this);
        Destroy(gameObject);
    }

}
