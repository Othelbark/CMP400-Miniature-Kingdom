using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : TooltipedObject
{
    protected KingdomManager _kingdomManager;

    [SerializeField] //TODO: refactor non-construction buildings to make use of this
    protected List<Agent> _assignedAgents;

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


    public virtual int GetMaxAssignedAgents()
    {
        return int.MaxValue;
    }
    //Only call from Agent
    public bool AddAgent(Agent agent, bool forceAdd = false)
    {
        if (_assignedAgents.Count < GetMaxAssignedAgents())
        {
            _assignedAgents.Add(agent);
            return true;
        }
        if (forceAdd)
        {
            if (_assignedAgents.Count > 0)
            {
                _assignedAgents[0].SetTargetBuilding(null);
            }
            _assignedAgents.Add(agent);
            return true;
        }
        return false;
    }
    //Only call from Agent
    public void RemoveAgent(Agent agent)
    {
        _assignedAgents.Remove(agent);
    }
    public bool HasAgent(Agent agent)
    {
        return _assignedAgents.Contains(agent);
    }
    public bool CanTakeMoreAgents()
    {
        if (_assignedAgents.Count < GetMaxAssignedAgents())
        {
            return true;
        }
        return false;
    }
    public int GetAssignedAgentCount()
    {
        return _assignedAgents.Count;
    }


}
