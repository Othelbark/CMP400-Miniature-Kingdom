using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipedObject : MonoBehaviour
{
    [SerializeField]
    protected string _tooltipName = "";
    [SerializeField]
    protected string _tooltipText;

    public virtual string GetText()
    {
        string fullText = "";

        if (_tooltipName == "")
        {
            fullText += "<b>" + gameObject.name + "</b>\n";
        }
        else
        {
            fullText += "<b>" + _tooltipName + "</b>\n";
        }

        fullText += _tooltipText;

        return fullText;
    }
}
