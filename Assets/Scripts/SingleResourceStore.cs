using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleResourceStore : Building
{
    public ResourceType resourceType = ResourceType.NONE;

    [SerializeField]
    private float _currentResources = 0.0f;

    [SerializeField]
    private float _capacity = 100.0f;


    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

   public void SetCurrentResources(float r)
    {
        _currentResources = r;

        if (_currentResources > _capacity)
            _currentResources = _capacity;
    }

    // Returns leftover resoucres if capacity is reached
    public float AddResources(float r)
    {
        if ((_capacity - _currentResources) >= r)
        {
            _currentResources += r;
            return 0;
        }
        else
        {
            float leftoverResources = r - (_capacity - _currentResources);
            _currentResources = _capacity;
            return leftoverResources;
        }
    }

    public float TakeResources(float r)
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
