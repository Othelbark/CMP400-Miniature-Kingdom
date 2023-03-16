using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipedObject : MonoBehaviour
{
    [SerializeField]
    protected string _tooltipText;

    public virtual string GetText()
    {
        string fullText = "<b>" + gameObject.name + "</b>\n";
        fullText += _tooltipText;
        return fullText;
    }
}
