using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionSystem : MonoBehaviour
{
    public ControlType type { get; protected set; } = ControlType.NONE;

    protected bool _active = false;

    // Start is called before the first frame update
    public void Start()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {
        
    }

    public virtual void SetActive(bool b) { _active = b; }

    public bool IsActive() { return _active; }
}
