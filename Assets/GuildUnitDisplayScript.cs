using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuildUnitDisplayScript : MonoBehaviour
{

    public int unitCount;
    public int activeUnitCount;
    public int maxUnitCount;

    public float buffer = 5;

    protected int _lastUnityCount;
    protected int _lastActiveUnitCount;
    protected int _lastMaxUnitCount;

    [SerializeField]
    protected GameObject _activeUnitPrefab;
    [SerializeField]
    protected GameObject _inactiveUnitPrefab;
    [SerializeField]
    protected GameObject _emptyUnitPrefab;

    [SerializeField]
    protected List<GameObject> _display;

    protected RectTransform _transform;


    // Start is called before the first frame update
    void Start()
    {
        if (_activeUnitPrefab.GetComponent<RectTransform>().localScale != _inactiveUnitPrefab.GetComponent<RectTransform>().localScale)
        {
            Debug.LogError("Scale of active and inactive unit displays inconsistent");
        }

        _transform = GetComponent<RectTransform>();

        _display = new List<GameObject>();

        _lastUnityCount = unitCount;
        _lastActiveUnitCount = activeUnitCount;
        _lastMaxUnitCount = maxUnitCount;

        UpdateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        if (unitCount != _lastUnityCount || activeUnitCount != _lastActiveUnitCount || maxUnitCount != _lastMaxUnitCount)
        {
            _lastUnityCount = unitCount;
            _lastActiveUnitCount = activeUnitCount;
            _lastMaxUnitCount = maxUnitCount;


            UpdateDisplay();
        }

    }

    void UpdateDisplay()
    {
        foreach (GameObject o in _display)
        {
            Destroy(o);
        }
        _display.Clear();

        float unitSize = (_transform.rect.width - (buffer * (maxUnitCount - 1) ) ) / maxUnitCount;

        if (unitSize <= 0)
        {
            Debug.LogWarning("Buffer to large of Guild Unit display");
            buffer = 0;
            unitSize = _transform.rect.width / maxUnitCount;
        }

        for (int i = 0; i < maxUnitCount; i++)
        {
            if (i < activeUnitCount)
            {
                GameObject activeUnit = Instantiate(_activeUnitPrefab, _transform);
                RectTransform activeUnitTransform = activeUnit.GetComponent<RectTransform>();
                activeUnitTransform.anchoredPosition = new Vector2((unitSize + buffer) * i, 0);
                activeUnitTransform.sizeDelta = new Vector2(unitSize / activeUnitTransform.localScale.x, activeUnitTransform.rect.height);

                _display.Add(activeUnit);
            }
            else if (i < unitCount)
            {
                GameObject inactiveUnit = Instantiate(_inactiveUnitPrefab, _transform);
                RectTransform inactiveUnitTransform = inactiveUnit.GetComponent<RectTransform>();
                inactiveUnitTransform.anchoredPosition = new Vector2((unitSize + buffer) * i, 0);
                inactiveUnitTransform.sizeDelta = new Vector2(unitSize / inactiveUnitTransform.localScale.x, inactiveUnitTransform.rect.height);

                _display.Add(inactiveUnit);
            }
            else
            {
                GameObject emptyUnit = Instantiate(_emptyUnitPrefab, _transform);
                RectTransform emptyUnitTransform = emptyUnit.GetComponent<RectTransform>();
                emptyUnitTransform.anchoredPosition = new Vector2((unitSize + buffer) * i, 0);
                emptyUnitTransform.sizeDelta = new Vector2(unitSize / emptyUnitTransform.localScale.x, emptyUnitTransform.rect.height);

                _display.Add(emptyUnit);
            }
        }
    }
}
