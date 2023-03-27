using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenuScript : MonoBehaviour
{
    public PrefabsByString constructions;

    protected GameObject _selectedPrefab = null;

    [SerializeField]
    protected GameObject _constructionDisplay;
    protected SpriteRenderer _constructionDisplaySR;

    private void Awake()
    {
        _constructionDisplaySR = _constructionDisplay.GetComponentInChildren<SpriteRenderer>();
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
            _constructionDisplay.SetActive(true);

            _constructionDisplaySR.sprite = _selectedPrefab.GetComponent<Construction>().buildingPrefab.GetComponentInChildren<SpriteRenderer>().sprite;
            _constructionDisplaySR.transform.localPosition = _selectedPrefab.GetComponentInChildren<SpriteRenderer>().transform.localPosition;

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            _constructionDisplay.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0);

        }
        else
        {
            _constructionDisplay.SetActive(false);
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
