using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gatherable : MonoBehaviour
{
    public ResourceType resourceType = ResourceType.NONE;

    [SerializeField]
    private float _currentResources = 0.0f;

    // Reference to the Natural World Manager
    private NaturalWorldManager _naturalWorldManager;

    // Start is called before the first frame update
    void Start()
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCurrentResources(float r)
    {
        _currentResources = r;
    }

    public void AddResources(float r)
    {
        _currentResources += r;
    }

    public float HarvestResources(float r)
    {
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
