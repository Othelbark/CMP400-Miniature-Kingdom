using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaturalWorldManager : MonoBehaviour
{
    [SerializeField]
    private List<Gatherable> _gatherables;

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
}
