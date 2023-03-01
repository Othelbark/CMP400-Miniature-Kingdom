using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfSpawningGatherable : GrowableGatherable
{
    [SerializeField]
    protected float _minSpawnTime = 5;
    [SerializeField]
    protected float _maxSpwanTime = 15;

    protected float _spawnTimer = 0.0f;

    [SerializeField]
    protected float _minSpawnDistance = 0.5f;
    [SerializeField]
    protected float _maxSpawnDistance = 1.0f;

    [SerializeField]
    protected GameObject _prefabToSpawn;

    [SerializeField]
    protected bool _spawnOnlyWhenFullyGrown = true;

    // Start is called before the first frame update
    new public void Start()
    {
        base.Start();

        _spawnTimer = Random.Range(_minSpawnTime, _maxSpwanTime);
    }

    // Update is called once per frame
    new public void Update()
    {
        base.Update();

        //if fully grown or not limited
        if (_totalGrowth >= _maxGrowth || !_spawnOnlyWhenFullyGrown)
        {
            _spawnTimer -= Time.deltaTime;

            if (_spawnTimer <= 0.0f)
            {
                _spawnTimer += Random.Range(_minSpawnTime, _maxSpwanTime);
                SpawnChild();
            }
        }
    }


    protected void SpawnChild()
    {
        float distance = Random.Range(_minSpawnDistance, _maxSpawnDistance);
        Vector3 direction = Random.insideUnitCircle.normalized;

        Vector3 childPos = transform.position + (direction * distance);
        if (_naturalWorldManager.CanSpawn(childPos))
        {
            GameObject child = Instantiate(_prefabToSpawn, _naturalWorldManager.transform);

            child.transform.position = childPos;
        }
    }

    protected void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("Collision");
        if (collision.gameObject.GetComponent<Gatherable>())
        {
            Debug.Log("with gatherable.");
            Spent();
        }
    }
}
