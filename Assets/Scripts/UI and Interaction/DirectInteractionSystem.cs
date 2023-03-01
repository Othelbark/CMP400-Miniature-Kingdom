using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectInteractionSystem : PlayerInteractionSystem
{
    [SerializeField]
    protected GameObject _selectedAgentHighlighter;

    protected Agent _selectedAgent = null;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        type = ControlType.DIRECT;
    }

    // Update is called once per frame
    void Update()
    {
        if (_active)
        {

            //Detect clicks on agents
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D clicked; 
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                if (clicked = Physics2D.Raycast(mousePos2D, Vector2.zero))
                {
                    Agent agent;
                    if (agent = clicked.transform.gameObject.GetComponent<Agent>())
                    {
                        if (_selectedAgent == null)
                            _selectedAgentHighlighter.SetActive(true);

                        _selectedAgent = agent;
                    }
                }
                else
                {
                    if (_selectedAgent != null)
                        _selectedAgentHighlighter.SetActive(false);

                    _selectedAgent = null;
                }
            }

            
            if (Input.GetMouseButtonDown(1) && _selectedAgent)
            {

                RaycastHit2D clicked;
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                if (clicked = Physics2D.Raycast(mousePos2D, Vector2.zero))
                {
                    Gatherable gatherable;
                    if (gatherable = clicked.transform.gameObject.GetComponent<Gatherable>())
                    {
                        Guild gatherersGuild = GameObject.FindGameObjectWithTag(gatherable.resourceType + "GatherersGuild").GetComponent<Guild>();
                        _selectedAgent.SetGuild(gatherersGuild);
                        _selectedAgent.state = AgentState.WAITING;
                    }

                    ResourceProcessor processor;
                    if (processor = clicked.transform.gameObject.GetComponent<ResourceProcessor>())
                    {
                        Guild processorGuild = processor.guild;
                        processorGuild.ClearAgents();
                        _selectedAgent.SetGuild(processor.guild);
                        _selectedAgent.state = AgentState.WAITING;
                    }
                }
                else
                {
                    //Right clicked on nothing
                    _selectedAgent.RemoveFromGuild();
                    _selectedAgent.SetMovingTowards(mousePos2D, 0.0f, true);
                }
            }


        }
    }

    void LateUpdate()
    {
        if (_selectedAgent)
        {
            _selectedAgentHighlighter.transform.position = _selectedAgent.transform.position;
        }
    }

    public override void SetActive(bool b) 
    {
        base.SetActive(b);

        if (b)
        {
            //Activation
            //Debug.Log(type + " Control System Activated.");
        }
        else
        {
            //Disactivation
            //Debug.Log(type + " Control System Disactivated.");
            _selectedAgent = null;
            _selectedAgentHighlighter.SetActive(false);
        }
    }
}
