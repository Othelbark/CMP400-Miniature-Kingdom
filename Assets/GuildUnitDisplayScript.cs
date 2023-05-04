using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuildUnitDisplayScript : MonoBehaviour
{

    public int unitCount;
    public int activeUnitCount;
    public int maxUnitCount;

    protected int _lastUnityCount;
    protected int _lastActiveUnitCount;
    protected int _lastMaxUnitCount;

    [SerializeField]
    protected GameObject _activeUnitPrefab;
    [SerializeField]
    protected GameObject _inactiveUnitPrefab;

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

        float unitSize = _transform.rect.width / maxUnitCount;

        for (int i = 0; i < unitCount; i++)
        {
            if (i < activeUnitCount)
            {
                GameObject activeUnit = Instantiate(_activeUnitPrefab, _transform);
                RectTransform activeUnitTransform = activeUnit.GetComponent<RectTransform>();
                activeUnitTransform.anchoredPosition = new Vector2(unitSize * i, 0);
                activeUnitTransform.sizeDelta = new Vector2(unitSize / activeUnitTransform.localScale.x, activeUnitTransform.rect.height);

                _display.Add(activeUnit);
            }
            else
            {
                GameObject inactiveUnit = Instantiate(_inactiveUnitPrefab, _transform);
                RectTransform inactiveUnitTransform = inactiveUnit.GetComponent<RectTransform>();
                inactiveUnitTransform.anchoredPosition = new Vector2(unitSize * i, 0);
                inactiveUnitTransform.sizeDelta = new Vector2(unitSize / inactiveUnitTransform.localScale.x, inactiveUnitTransform.rect.height);

                _display.Add(inactiveUnit);
            }
        }
    }
}
