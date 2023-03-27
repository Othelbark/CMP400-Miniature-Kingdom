using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonToggle(string button, bool on)
    {
        if (on)
            Debug.Log(button + " turned on");
        else
            Debug.Log(button + " turned off");
    }
}
