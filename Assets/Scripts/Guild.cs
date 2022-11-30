using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guild : MonoBehaviour
{
    public GuildState state = GuildState.ACTIVE;

    public int targetAgentCount = 1;


    [SerializeField] //Temporalily Serialized for testing
    protected List<Agent> _agents;

    protected KingdomManager _kingdomManager;

    protected NaturalWorldManager _naturalWorldManager;

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

        _kingdomManager.AddGuild(this);


        try
        {
            _naturalWorldManager = GameObject.FindGameObjectWithTag("NaturalWorldManager").GetComponent<NaturalWorldManager>();
        }
        catch
        {
            Debug.LogError("Can't find natural world manager.");
        }
    }

    // Update is called once per frame
    public void Update()
    {
        if (state == GuildState.INACTIVE && _agents.Count > 0)
        {
            while (_agents.Count > 0)
            {
                _agents[0].RemoveFromGuild();
            }
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
}
