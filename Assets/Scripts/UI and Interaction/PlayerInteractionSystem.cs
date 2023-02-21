using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionSystem : MonoBehaviour
{
    public ControlType type { get; protected set; } = ControlType.NONE;

    protected bool _active = false;

    protected Camera _mainCamera;

    // Start is called before the first frame update
    public void Start()
    {
        _mainCamera = Camera.main;
    }

    public virtual void SetActive(bool b) { _active = b; }

    public bool IsActive() { return _active; }
}
