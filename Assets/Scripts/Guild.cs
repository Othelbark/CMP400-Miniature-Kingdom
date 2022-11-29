using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guild : MonoBehaviour
{
    [SerializeField] //Temporalily Serialized for testing
    private List<Agent> _agents;

    private KingdomManager _kingdomManager;

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

        _kingdomManager.AddGuild(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddAgent(Agent agent)
    {
        _agents.Add(agent);
    }

    public void RemoveAgent(Agent agent)
    {
        _agents.Remove(agent);
    }
}
