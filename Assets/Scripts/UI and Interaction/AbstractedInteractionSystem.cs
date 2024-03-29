using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractedInteractionSystem : PlayerInteractionSystem
{
    public GameObject AbstractedUI;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        type = ControlType.ABSTRACTED;
    }

    // Update is called once per frame
    void Update()
    {
        if (_active)
        {
            //Do stuff
        }
    }

    public override void SetActive(bool b)
    {
        base.SetActive(b);

        if (b)
        {
            //Activation
            //Debug.Log(type + " Control System Activated.");
            AbstractedUI.SetActive(true);
        }
        else
        {
            //Disactivation
            //Debug.Log(type + " Control System Disactivated.");
            AbstractedUI.SetActive(false);
        }
    }
}
