using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : TooltipedObject
{
    protected KingdomManager _kingdomManager;

    // Start is called before the first frame update
    public void Start()
    {
        try
        {
            _kingdomManager = GameObject.FindGameObjectWithTag("KingdomManager").GetComponent<KingdomManager>();
        }
        catch
        {
            Debug.LogError("Can't find kingdom manager.");
        }

        _kingdomManager.AddBuilding(this);
    }

    // Update is called once per frame
    public void Update()
    {
        
    }
}
