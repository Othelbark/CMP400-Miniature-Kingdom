using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingdomManager : MonoBehaviour
{
    [SerializeField] //Temporalily Serialized for testing
    private List<Guild> _guilds;

    [SerializeField] //Temporalily Serialized for testing
    private List<Agent> _agents;

    [SerializeField] //Temporalily Serialized for testing
    private List<Building> _buildings;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddGuild(Guild guild)
    {
        _guilds.Add(guild);
    }

    public void RemoveGuild(Guild guild)
    {
        _guilds.Remove(guild);
    }

    public void AddAgent(Agent agent)
    {
        _agents.Add(agent);
    }

    public void RemoveAgent(Agent agent)
    {
        _agents.Remove(agent);
    }

    public void AddBuilding(Building building)
    {
        _buildings.Add(building);
    }

    public void RemoveBuilding(Building building)
    {
        _buildings.Remove(building);
    }
}
