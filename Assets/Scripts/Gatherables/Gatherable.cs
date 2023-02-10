using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Gatherable : MonoBehaviour
{
    [HideInInspector]
    public GatherableState state = GatherableState.GATHERABLE_READY;
    public ResourceType resourceType = ResourceType.NONE;

    [SerializeField]
    protected float _currentResources = 0.0f;
    [SerializeField]
    protected bool _infiniteResorces = false;

    // Reference to the Natural World Manager
    protected NaturalWorldManager _naturalWorldManager;

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

        _naturalWorldManager.AddGatherable(this);

        state = GatherableState.GATHERABLE_READY;
    }

    // Update is called once per frame
    public void Update()
    {
        
    }

    public float GetCurrentResources()
    {
        return _currentResources;
    }

    public void SetCurrentResources(float r)
    {
        _currentResources = r;
    }

    public void AddResources(float r)
    {
        _currentResources += r;
    }

    //Returns amount actually harvested
    public virtual float HarvestResources(float r)
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
            return r;
        }
        else
        {
            float leftoverResources = _currentResources;
            _currentResources = 0;
            return leftoverResources;
        }
    }
}
