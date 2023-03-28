using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using ExtensionMethods;

namespace ExtensionMethods
{
    public static class ComponentExtensions
    {
        public static T GetCopyOf<T>(this Component comp, T other) where T : Component
        {
            Type type = comp.GetType();
            if (type != other.GetType()) return null; // type mis-match
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            PropertyInfo[] pinfos = type.GetProperties(flags);
            foreach (var pinfo in pinfos)
            {
                if (pinfo.CanWrite)
                {
                    try
                    {
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                    catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                }
            }
            FieldInfo[] finfos = type.GetFields(flags);
            foreach (var finfo in finfos)
            {
                finfo.SetValue(comp, finfo.GetValue(other));
            }
            return comp as T;
        }
        public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
        {
            return go.AddComponent<T>().GetCopyOf(toAdd) as T;
        }
    }
}

[RequireComponent(typeof(ToggleGroup))]
public class BuildMenuScript : MonoBehaviour
{
    public PrefabsByString constructions;

    protected GameObject _selectedPrefab = null;
    protected GameObject _lastSelectedPrefab = null;

    [SerializeField]
    protected ConstructionDisplayScript _constructionDisplay;
    protected SpriteRenderer _constructionDisplaySR;

    protected ToggleGroup _toggleGroup;

    private void Awake()
    {
        _constructionDisplaySR = _constructionDisplay.GetComponentInChildren<SpriteRenderer>();

        _toggleGroup = GetComponent<ToggleGroup>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_selectedPrefab != null)
        {
            if (_selectedPrefab != _lastSelectedPrefab)
            {
                _constructionDisplay.gameObject.SetActive(true);

                _constructionDisplaySR.sprite = _selectedPrefab.GetComponent<Construction>().buildingPrefab.GetComponentInChildren<SpriteRenderer>().sprite;
                _constructionDisplaySR.transform.localPosition = _selectedPrefab.GetComponentInChildren<SpriteRenderer>().transform.localPosition;

                Destroy(_constructionDisplay.GetComponent<PolygonCollider2D>());
                _constructionDisplay.gameObject.AddComponent<PolygonCollider2D>(_selectedPrefab.GetComponent<PolygonCollider2D>());

                _lastSelectedPrefab = _selectedPrefab;
            }

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _constructionDisplay.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0);


            if (Input.GetMouseButtonDown(0) && !_constructionDisplay.GetOverlapping())
            {
                GameObject construction = GameObject.Instantiate(_selectedPrefab);
                construction.transform.SetParent(GameObject.FindGameObjectWithTag("BuildingParent").transform);
                construction.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0);
                _toggleGroup.SetAllTogglesOff();
            }

        }
        else
        {
            _constructionDisplay.gameObject.SetActive(false);
        }
    }

    public void ButtonToggle(string button, bool on)
    {
        if (on && constructions.ContainsKey(button))
        {
            _selectedPrefab = constructions[button];
        }
        else
        {
            _selectedPrefab = null;
        }
    }
}
