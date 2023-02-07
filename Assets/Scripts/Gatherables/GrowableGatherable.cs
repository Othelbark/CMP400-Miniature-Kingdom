using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowableGatherable : Gatherable
{
    [SerializeField]
    protected float _growthRate = 1.0f;
    [SerializeField]
    [Tooltip("Maximum about grow. If InfiniteGrowth is true: max amount to grow to.")]
    protected float _maxGrowth = 100.0f;
    [SerializeField]
    protected bool _infiniteGrowth = false;

    protected float _totalGrowth = 0.0f;

    [SerializeField]
    [Tooltip("The minimum growth threshold to be gatherable")]
    protected float _minGrowth = 0.0f;
    [SerializeField]
    [Tooltip("(Only functions if InfinteGrowth = false) If growth should stop on first harvest")]
    protected bool _stopGrowthOnHarvest = false;

    protected bool _isGrowing = true;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        if (_totalGrowth < _minGrowth)
        {
            state = GatherableState.NON_GATHERABLE;
        }
        else if (_stopGrowthOnHarvest)
        {
            state = GatherableState.GATHERABLE_NOT_READY;
        }
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if (_isGrowing)
        {
            if (_infiniteGrowth)
            {
                _currentResources += Mathf.Min(_growthRate * Time.deltaTime, _maxGrowth - _currentResources);
                _totalGrowth += Mathf.Min(_growthRate * Time.deltaTime, _maxGrowth - _currentResources);
            }
            else if (_totalGrowth < _maxGrowth)
            {
                _currentResources += Mathf.Min(_growthRate * Time.deltaTime, _maxGrowth - _totalGrowth);
                _totalGrowth += Mathf.Min(_growthRate * Time.deltaTime, _maxGrowth - _totalGrowth);
            }


            switch (state)
            {
                case GatherableState.NON_GATHERABLE:
                    if (!_infiniteGrowth && _totalGrowth >= _minGrowth)
                    {
                        state = (_stopGrowthOnHarvest && !_infiniteGrowth) ? GatherableState.GATHERABLE_NOT_READY : GatherableState.GATHERABLE_READY;
                    }
                    else if (_infiniteGrowth && _currentResources >= _minGrowth)
                    {
                        state = GatherableState.GATHERABLE_READY;
                    }
                    break;
                case GatherableState.GATHERABLE_NOT_READY:
                    if (_totalGrowth >= _maxGrowth)
                    {
                        state = GatherableState.GATHERABLE_READY;
                    }
                    break;
                case GatherableState.GATHERABLE_READY:
                    if (_infiniteGrowth && _currentResources < 1.0f)
                    {
                        state = GatherableState.NON_GATHERABLE;
                    }
                    break;
            }
        }
    }

    public override float HarvestResources(float r)
    {
        if (_stopGrowthOnHarvest && !_infiniteGrowth)
        {
            _isGrowing = false;
            state = GatherableState.GATHERABLE_READY;
        }

        return base.HarvestResources(r);
    }

}
