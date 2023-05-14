using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrioritySlider : MonoBehaviour
{
    public string priorityName;

    protected KingdomManager _kingdomManager;

    protected Guild _guild;

    protected Slider _slider;

    protected GuildUnitDisplayScript _unitDisplay = null;

    public void Start()
    {
        _slider = GetComponent<Slider>();
        _kingdomManager = GameObject.FindGameObjectWithTag("KingdomManager").GetComponent<KingdomManager>();

        _kingdomManager.SetPriority(priorityName, _slider.value);

        _guild = _kingdomManager.GetGuildByPriorityName(priorityName);

        _unitDisplay = GetComponentInChildren<GuildUnitDisplayScript>();
    }

    public void Update()
    {
        if (_guild == null)
        {
            _guild = _kingdomManager.GetGuildByPriorityName(priorityName);
        }

        if (_unitDisplay != null)
        {

            int agentCountAtFull = Mathf.Max(Mathf.FloorToInt(_kingdomManager.GetAgentCount() * (1.0f / _kingdomManager.GetTotalPriority())), _kingdomManager.GetMaxAgentCountInGuilds());

            _unitDisplay.maxUnitCount = agentCountAtFull;


            if (_guild != null)
            {
                _unitDisplay.unitCount = _guild.GetCurrentAgentCount();

                if (_guild.state == GuildState.INACTIVE)
                {
                    _unitDisplay.activeUnitCount = 0;
                }
                else
                {
                    //TODO: more granular

                    _unitDisplay.activeUnitCount = _unitDisplay.unitCount;
                }
            }
            else
            {
                _unitDisplay.unitCount = 0;
                _unitDisplay.activeUnitCount = 0;
            }
        }
    }

    public void SetPriority(float p)
    {
        KingdomManager km = GameObject.FindGameObjectWithTag("KingdomManager").GetComponent<KingdomManager>();

        km.SetPriority(priorityName, p);
    }
}
