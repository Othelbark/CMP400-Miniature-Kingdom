using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guild : MonoBehaviour
{
    public GuildState state = GuildState.ACTIVE;

    public int targetAgentCount = 1;


    [SerializeField] //Temporalily Serialized for testing
    protected List<Agent> _agents;

    protected InteractionSystemController _interactionSystemController;
    protected KingdomManager _kingdomManager;
    protected NaturalWorldManager _naturalWorldManager;

    protected Dictionary<AgentState, bool> _guildTaskValidity;

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
    }

    // Update is called once per frame
    public void Update()
    {
        if (_interactionSystemController.GetControlType() == ControlType.ABSTRACTED)
        {
            UpdateTargetAgentCount();

            while (_agents.Count > targetAgentCount)
            {
                _agents[0].RemoveFromGuild();
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

                if (_agents.Count > 0)
                {
                    ClearAgents();
                }

                break;
        }
    }


    protected virtual void InitaliseGuildTaskValidity()
    {
        //Set it up so HasActivity always returns true when this and CheckTasks are not overriden
        _guildTaskValidity = new Dictionary<AgentState, bool>();
        _guildTaskValidity.Add(AgentState.WAITING, true);
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

    }
    protected virtual void ActiveUpdate()
    {

    }
    protected virtual void InactiveUpdate()
    {

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
