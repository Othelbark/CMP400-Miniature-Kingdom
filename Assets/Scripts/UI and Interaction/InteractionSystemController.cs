using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSystemController : MonoBehaviour
{
    [SerializeField]
    protected List<PlayerInteractionSystem> _playerInteractionSystems;
    [SerializeField]
    protected int _activeIS = 0;

    [SerializeField]
    protected GameObject _pauseMenu;
    protected bool _paused = false;
    protected float _activeTimeScale = 1.0f;

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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _paused = !_paused;

            if (_paused)
            {
                Time.timeScale = 0;
                _pauseMenu.SetActive(true);
            }
            else
            {
                Time.timeScale = _activeTimeScale;
                _pauseMenu.SetActive(false);
            }
        }
    }

    public ControlType GetControlType() { return _playerInteractionSystems[_activeIS].type; }

    public void SetTimeScale(float ts)
    {
        _activeTimeScale = ts;
        if (!_paused)
            Time.timeScale = _activeTimeScale;
    }
}
