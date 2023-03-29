using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class BuildingConstructedEvent : UnityEvent<string, Building> { }

public class InteractionSystemController : MonoBehaviour
{
    [SerializeField]
    protected List<PlayerInteractionSystem> _playerInteractionSystems;
    [SerializeField]
    protected int _activeIS = 0;

    [SerializeField]
    protected GameObject _loseMenu;
    [SerializeField]
    protected GameObject _winMenu;
    [SerializeField]
    protected GameObject _pauseMenu;

    [SerializeField]
    protected Text _timerDisplay;

    [SerializeField]
    protected float _timeLimit = 300;
    protected float _timeRemaining;
    [SerializeField]
    protected int _foodInCastleToWin = 5000;

    protected bool _paused = false;
    protected float _activeTimeScale = 1.0f;

    protected bool _gameOver;

    [HideInInspector]
    public BuildingConstructedEvent buildingConstructedEvent;
    protected ResourceStore _castle = null;

    void Awake()
    {

        if (buildingConstructedEvent == null)
        {
            buildingConstructedEvent = new BuildingConstructedEvent();
        }

        buildingConstructedEvent.AddListener(BuildingConstructed);
    }

    // Start is called before the first frame update
    void Start()
    {
        _loseMenu.SetActive(false);
        _winMenu.SetActive(false);

        _timeRemaining = _timeLimit;
        _playerInteractionSystems[_activeIS].SetActive(true);

        Time.timeScale = _activeTimeScale;
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

        if (!_gameOver)
        {
            _timeRemaining -= Time.deltaTime;

            if (_timeRemaining <= 0)
            {
                _timeRemaining = 0;
                _loseMenu.SetActive(true);
                _activeTimeScale = 0;
                Time.timeScale = 0;
                _gameOver = true;
            }

            int minutesRemaining = Mathf.FloorToInt(_timeRemaining / 60F);
            int secondsRemaining = Mathf.FloorToInt(_timeRemaining - minutesRemaining * 60);
            string niceTime = string.Format("{0:0}:{1:00}", minutesRemaining, secondsRemaining);

            _timerDisplay.text = "Time Remaining: " + niceTime;


            if (_castle != null)
            {
                if (_castle.GetAmount(ResourceType.FOOD) >= _foodInCastleToWin)
                {
                    WinGame();
                }
            }

        }
        else
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                int scene = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(scene, LoadSceneMode.Single);
            }
        }
    }

    public ControlType GetControlType() { return _playerInteractionSystems[_activeIS].type; }

    public void SetTimeScale(float ts)
    {
        if (!_gameOver)
        {
            _activeTimeScale = ts;
            if (!_paused)
                Time.timeScale = _activeTimeScale;
        }
    }

    void BuildingConstructed(string name, Building building)
    {
        if (name == "Castle")
        {
            if ((_castle = building as ResourceStore) != null)
            {
                if (!_castle.HasType(ResourceType.FOOD))
                {
                    Debug.LogError("Castle cannot store food");
                }
                else
                {
                    if (_castle.GetSpace() < _foodInCastleToWin)
                    {
                        Debug.LogError("Castle cannot store enough food");
                    }
                }
            }
            else
            {
                Debug.LogError("Castle is not a ResourceStore");
            }
        }
    }

    void WinGame()
    {
        if (!_gameOver)
        {
            _winMenu.SetActive(true);
            _activeTimeScale = 0;
            Time.timeScale = 0;
            _gameOver = true;
        }
    }

    public float GetTimeRemaining()
    {
        return _timeRemaining;
    }
}
