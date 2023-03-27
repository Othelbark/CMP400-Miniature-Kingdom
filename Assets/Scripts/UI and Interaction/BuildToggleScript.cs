using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Toggle))]
public class BuildToggleScript : TooltipedObject
{
    private Toggle _toggle;

    private BuildMenuScript _buildMenu;

    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
        _buildMenu = GetComponentInParent<BuildMenuScript>();
    }

    private void OnEnable()
    {
        _toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnDisable()
    {
        _toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool b)
    {
        _buildMenu.ButtonToggle(_tooltipName, b);
    }
}