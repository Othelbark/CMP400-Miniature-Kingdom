using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrioritySlider : MonoBehaviour
{
    public string priorityName;

    public void Start()
    {
        KingdomManager km = GameObject.FindGameObjectWithTag("KingdomManager").GetComponent<KingdomManager>();

        km.SetPriority(priorityName, 0.5f);
    }

    public void SetPriority(float p)
    {
        KingdomManager km = GameObject.FindGameObjectWithTag("KingdomManager").GetComponent<KingdomManager>();

        km.SetPriority(priorityName, p);
    }
}
