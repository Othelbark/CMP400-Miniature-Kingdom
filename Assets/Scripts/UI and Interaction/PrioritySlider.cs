using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrioritySlider : MonoBehaviour
{
    public string priorityName;

    protected Slider _slider;

    public void Start()
    {
        _slider = GetComponent<Slider>();
        KingdomManager km = GameObject.FindGameObjectWithTag("KingdomManager").GetComponent<KingdomManager>();

        km.SetPriority(priorityName, _slider.value);
    }

    public void SetPriority(float p)
    {
        KingdomManager km = GameObject.FindGameObjectWithTag("KingdomManager").GetComponent<KingdomManager>();

        km.SetPriority(priorityName, p);
    }
}
