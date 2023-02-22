using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrioritySlider : MonoBehaviour
{
    public string priorityName;

    public void SetPriority(float p)
    {
        KingdomManager km = GameObject.FindGameObjectWithTag("KingdomManager").GetComponent<KingdomManager>();

        km.SetPriority(priorityName, p);
    }
}
