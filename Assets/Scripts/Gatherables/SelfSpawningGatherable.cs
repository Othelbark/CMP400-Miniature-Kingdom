using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfSpawningGatherable : GrowableGatherable
{
    [SerializeField]
    protected float _minSpawnTime = 5;
    [SerializeField]
    protected float _maxSpwanTime = 15;


    [SerializeField]
    protected float _minSpawnDistance = 0.5f;
    [SerializeField]
    protected float _maxSpawnDistance = 1.0f;

    [SerializeField]
    protected SelfSpawningGatherable _prefabToSpawn;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }
}
