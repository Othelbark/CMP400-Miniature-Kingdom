using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Gatherable : TooltipedObject
{
    [HideInInspector]
    public GatherableState state = GatherableState.GATHERABLE_READY;
    public ResourceType resourceType = ResourceType.NONE;

    [SerializeField]
    protected int _currentResources = 0;
    [SerializeField]
    protected bool _infiniteResorces = false;

    [SerializeField]
    protected List<Agent> _gatherers;
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

        _gatherers = new List<Agent>();

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
        if (_gatherers.Count < _maxGatherers)
        {
            _gatherers.Add(gatherer);
            return true;
        }
        if (forceAdd)
        {
            _gatherers[0].SetTargetGatherable(null);
            _gatherers.Add(gatherer);
            return true;
        }
        return false;
    }
    public void RemoveGatherer(Agent gatherer)
    {
        _gatherers.Remove(gatherer);
    }
    public bool CanTakeMoreGatherers()
    {
        if (_gatherers.Count < _maxGatherers)
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
        foreach (Agent gatherer in _gatherers)
        {
            gatherer.ClearTargetGatherable();
        }

        _naturalWorldManager.RemoveGatherable(this);
        Destroy(gameObject);
    }

    public override string GetText(string additionalText = "")
    {
        string gatherableInfo = "\n";

        if (!_infiniteResorces)
        {
            gatherableInfo += "Current ";

            string resourceName = "";
            resourceName += resourceType;
            gatherableInfo += resourceName.ToLower();

            gatherableInfo += ": " + _currentResources;
        }
        else
        {
            gatherableInfo += "Never runs out";
        }

        return base.GetText(gatherableInfo);
    }

}
