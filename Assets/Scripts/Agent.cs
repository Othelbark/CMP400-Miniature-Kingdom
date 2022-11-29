using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [SerializeField]
    private InventoryDictionary _inventory;

    [SerializeField] //Temporalily Serialized for testing
    private float _totalInventory = 0.0f;

    [SerializeField]
    private float _capacity = 100.0f;

    private KingdomManager _kingdomManager;

    [SerializeField]
    private Guild _guild = null;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            _kingdomManager = GameObject.FindGameObjectWithTag("KingdomManager").GetComponent<KingdomManager>();
        }
        catch
        {
            Debug.LogError("Can't find kingdom manager.");
        }

        _kingdomManager.AddAgent(this);

        //Add to guild if one is set
        if (_guild != null)
        {
            _guild.AddAgent(this);
        }

        //Update total inventory if inital inventory set
        _totalInventory = 0.0f;
        foreach (KeyValuePair<ResourceType, float> item in _inventory)
        {
            _totalInventory += item.Value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGuild (Guild guild)
    {
        if (_guild != null)
        {
            RemoveFromGuild();
        }

        _guild = guild;
        _guild.AddAgent(this);
    }

    public Guild GetGuild ()
    {
        return _guild;
    }

    public void RemoveFromGuild ()
    {
        if (_guild != null)
        {
            _guild.RemoveAgent(this);
            _guild = null;
        }
    }

}
