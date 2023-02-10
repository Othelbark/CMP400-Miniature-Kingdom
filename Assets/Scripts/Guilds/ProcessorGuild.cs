using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessorGuild : Guild
{

    [SerializeField]
    protected ResourceProcessor _processor;

    [SerializeField] // Temp serialised
    protected InventoryDictionary _processorNeeds;

    protected float _minProcessingDistance = 0.2f;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        targetAgentCount = 1;
    }

    // Update is called once per frame
    new void Update()
    {
        if (_agents.Count > 1)
        {
            Debug.LogError("More than one agent assigned to a processor Guild");
        }
        if (!_processor)
        {
            Debug.LogError("No processor assigned to processor Guild");
        }

        if (state == GuildState.ACTIVE && _agents.Count > 0)
        {
            if (_agents[0].state == AgentState.WAITING)
            {
                _processorNeeds = _processor.GetNeeds();

                if (_processorNeeds.Count == 0)
                {
                    _agents[0].state = AgentState.WORKING;
                }
                else
                {
                    //TODO: state logic for collecting resources
                    state = GuildState.INACTIVE;
                }
            }

            if (_agents[0].state == AgentState.WORKING)
            {
                float distanceToProcessor = (_agents[0].transform.position - _processor.transform.position).magnitude;

                if (distanceToProcessor <= _minProcessingDistance)
                {
                    if(!_processor.Process(Time.deltaTime))
                    {
                        _agents[0].state = AgentState.WAITING;
                    }
                }
                else
                {
                    _agents[0].SetMovingTowards(_processor.transform.position, _minProcessingDistance);
                }
            }

        }

        base.Update();
    }
}
