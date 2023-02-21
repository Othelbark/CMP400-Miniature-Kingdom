using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSystemController : MonoBehaviour
{
    [SerializeField]
    protected List<PlayerInteractionSystem> _playerInteractionSystems;
    [SerializeField]
    protected int _activeIS = 0;

    // Start is called before the first frame update
    void Start()
    {
        _playerInteractionSystems[_activeIS].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _playerInteractionSystems[_activeIS].SetActive(false);

            _activeIS++;
            if (_activeIS >= _playerInteractionSystems.Count)
            {
                _activeIS = 0;
            }

            _playerInteractionSystems[_activeIS].SetActive(true);
        }
    }

    public ControlType GetControlType() { return _playerInteractionSystems[_activeIS].type; }
}
