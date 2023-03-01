using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowableGatherable : Gatherable
{
    [SerializeField]
    protected float _growthRate = 1.0f;
    [SerializeField]
    [Tooltip("Maximum about grow. If InfiniteGrowth is true: max amount to grow to.")]
    protected int _maxGrowth = 100;
    [SerializeField]
    protected bool _infiniteGrowth = false;

    [SerializeField]
    [Tooltip("The minimum growth threshold to be gatherable")]
    protected int _minGrowth = 0;
    [SerializeField]
    [Tooltip("(Only functions if InfinteGrowth = false) If growth should stop on first harvest")]
    protected bool _stopGrowthOnHarvest = false;

    protected bool _isGrowing = true;

    protected int _totalGrowth = 0;
    protected float _residualGrowth = 0.0f;

    // Start is called before the first frame update
    new public void Start()
    {
        base.Start();

        _totalGrowth = _currentResources;

        if (_totalGrowth < _minGrowth)
        {
            state = GatherableState.NON_GATHERABLE;
        }
        else if (_stopGrowthOnHarvest && _totalGrowth < _maxGrowth)
        {
            state = GatherableState.GATHERABLE_NOT_READY;
        }
    }

    // Update is called once per frame
    new public void Update()
    {
        base.Update();

        if (_isGrowing)
        {
            float growth = _growthRate * Time.deltaTime + _residualGrowth;
            int growthInt = Mathf.FloorToInt(growth);
            _residualGrowth = growth - growthInt;

            if (_infiniteGrowth)
            {
                _currentResources += Mathf.Min(growthInt, _maxGrowth - _currentResources);
                _totalGrowth += Mathf.Min(growthInt, _maxGrowth - _currentResources);
            }
            else if (_totalGrowth < _maxGrowth)
            {
                _currentResources += Mathf.Min(growthInt, _maxGrowth - _totalGrowth);
                _totalGrowth += Mathf.Min(growthInt, _maxGrowth - _totalGrowth);
            }


            switch (state)
            {
                case GatherableState.NON_GATHERABLE:
                    if (!_infiniteGrowth && _totalGrowth >= _minGrowth)
                    {
                        state = (_stopGrowthOnHarvest) ? GatherableState.GATHERABLE_NOT_READY : GatherableState.GATHERABLE_READY;
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
                        if (!_infiniteGrowth)
                            _isGrowing = false;
                    }
                    break;
                case GatherableState.GATHERABLE_READY:
                    if (_infiniteGrowth && _currentResources <= 0)
                    {
                        state = GatherableState.NON_GATHERABLE;
                    }
                    else if (!_infiniteGrowth && _totalGrowth >= _maxGrowth)
                    {
                        _isGrowing = false;
                    }
                    break;
            }
        }
    }

    public override int HarvestResources(int r)
    {
        if (_stopGrowthOnHarvest && !_infiniteGrowth)
        {
            _isGrowing = false;
            state = GatherableState.GATHERABLE_READY;
        }

        return base.HarvestResources(r);
    }

    protected override void CheckSpent()
    {

        if (_currentResources <= 0 && !_infiniteResorces && !_isGrowing)
        {
            Spent();
        }
    }
}
