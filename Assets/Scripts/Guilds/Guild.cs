using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guild : MonoBehaviour
{
    public GuildState state = GuildState.ACTIVE;

    public float targetAgentCount = 1;


    [SerializeField] //Temporalily Serialized for testing
    protected List<Agent> _agents;

    protected InteractionSystemController _interactionSystemController;
    protected KingdomManager _kingdomManager;
    protected NaturalWorldManager _naturalWorldManager;

    protected Dictionary<AgentState, bool> _guildTaskValidity; //Dictionary of all tasks that can be assigned from waiting and their current validity


    [SerializeField]
    protected float _minStoreDistance = 0.0f;
    [SerializeField]
    protected float _minInteractionDistance = 0.2f;

    public string _priorityName { get; protected set; }

    // Start is called before the first frame update
    public void Start()
    {
        if (_agents == null)
        {
            _agents = new List<Agent>();
        }


        try
        {
            _interactionSystemController = GameObject.FindGameObjectWithTag("InteractionSystemController").GetComponent<InteractionSystemController>();
        }
        catch
        {
            Debug.LogError("Can't find interaction system controller.");
        }

        try
        {
            _kingdomManager = GameObject.FindGameObjectWithTag("KingdomManager").GetComponent<KingdomManager>();
        }
        catch
        {
            Debug.LogError("Can't find kingdom manager.");
        }

        _kingdomManager.AddGuild(this);

        try
        {
            _naturalWorldManager = GameObject.FindGameObjectWithTag("NaturalWorldManager").GetComponent<NaturalWorldManager>();
        }
        catch
        {
            Debug.LogError("Can't find natural world manager.");
        }

        InitaliseGuildTaskValidity();

        SetPriorityName();
    }

    // Update is called once per frame
    public void Update()
    {
        if (_interactionSystemController.GetControlType() == ControlType.ABSTRACTED)
        {
            UpdateTargetAgentCount();

            //remove any agents assigned over the integer threshold
            while (_agents.Count > Mathf.CeilToInt(targetAgentCount))
            {
                _agents[0].RemoveFromGuild();
            }

            //check fractional overassingment against max deficet
            if (_agents.Count > targetAgentCount)
            {
                float deficetIfOneLess = targetAgentCount - (_agents.Count - 1);

                if (deficetIfOneLess < _kingdomManager.largestCurrentAgentDeficet)
                {
                    _agents[0].RemoveFromGuild();
                }
            }
        
        }

        CheckActivity();

        switch (state)
        {
            case GuildState.ACTIVE:

                ActiveUpdate();

                break;
            case GuildState.INACTIVE:

                InactiveUpdate();

                break;
        }
    }


    protected virtual void InitaliseGuildTaskValidity()
    {
        //Set it up so HasActivity always returns true when this and CheckTasks are not overriden
        _guildTaskValidity = new Dictionary<AgentState, bool>();
        _guildTaskValidity.Add(AgentState.WAITING, true);
    }
    protected virtual void SetPriorityName()
    {
        _priorityName = gameObject.name + Random.Range(10000, 99999);
    }


    protected void CheckActivity()
    {
        if (HasActivity())
        {
            state = GuildState.ACTIVE;
        }
        else
        {
            state = GuildState.INACTIVE;
        }
    }
    protected bool HasActivity()
    {
        CheckTasks();

        foreach (KeyValuePair<AgentState, bool> task in _guildTaskValidity)
        {
            if (task.Value == true)
            {
                return true;
            }
        }

        foreach (Agent agent in _agents)
        {
            if (agent.state != AgentState.WAITING)
            {
                return true;
            }
        }

        return false;
    }


    protected virtual void CheckTasks()
    {
    }


    protected virtual void UpdateTargetAgentCount()
    {
        float priorityFactor = _kingdomManager.GetPriority(_priorityName) / _kingdomManager.GetTotalPriority();

        targetAgentCount = (float)_kingdomManager.GetAgentCount() * priorityFactor;
    }
    protected virtual void ActiveUpdate()
    {

    }
    protected virtual void InactiveUpdate()
    {

    }


    protected void AssignWaitingAgent(Agent agent)
    {
        if (agent.state != AgentState.WAITING)
            Debug.LogError("Called \"AssignWaitingAgent\" on an agent not in waiting state.");

        CheckTasks();

        foreach (KeyValuePair<AgentState, bool> task in _guildTaskValidity)
        {
            if (task.Value)
            {
                agent.state = task.Key;
                return;
            }
        }

        agent.state = AgentState.WAITING;
    }
    protected void CollectFromStore(Agent agent, ResourceStore store, ResourceType type, int amount)
    {
        if ((store.transform.position - agent.transform.position).magnitude <= _minStoreDistance)
        {
            int pickedUp = store.TakeResources(type, amount);

            int leftover = agent.AddToInventory(type, pickedUp);

            if (leftover > 0)
            {
                store.AddResources(type, leftover);
            }
        }
        else
        {
            agent.SetMovingTowards(store.transform.position, _minStoreDistance);
        }
    }


    public int GetCurrentAgentCount()
    {
        return _agents.Count;
    }
    public void AddAgent(Agent agent)
    {
        _agents.Add(agent);
    }
    public void RemoveAgent(Agent agent)
    {
        _agents.Remove(agent);
    }
    public void ClearAgents()
    {
        while (_agents.Count > 0)
        {
            _agents[0].RemoveFromGuild();
        }
    }
}
