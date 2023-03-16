using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [SerializeField]
    protected Image _image;
    [SerializeField]
    protected Text _text;

    [SerializeField]
    protected Vector2 _offset;

    [SerializeField]
    protected float _verticalSpacing = 20.0f;
    [SerializeField]
    protected float _horizontalSpacing = 20.0f;
    [SerializeField]
    protected float _maxHorizontalSize = 200.0f;

    protected bool _active = false;


    public ContactFilter2D tooltipableFilter;

    // Start is called before the first frame update
    void Start()
    {
        _text.gameObject.SetActive(false);
        _image.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        if (Physics2D.Raycast(mousePos2D, Vector2.zero, tooltipableFilter, hits) > 0)
        {
            TooltipedObject tooltipedObject;
            if (tooltipedObject = hits[0].transform.gameObject.GetComponent<TooltipedObject>())
            {
                SetText(tooltipedObject.GetText());
                if (!_active)
                {
                    _active = true;
                    _text.gameObject.SetActive(true);
                    _image.gameObject.SetActive(true);
                }
            }
            else
            {
                Debug.LogWarning("No tooltip on object in Tooltipable filter");
            }
        }
        else
        {
            if (_active)
            {
                _active = false;
                _text.gameObject.SetActive(false);
                _image.gameObject.SetActive(false);
            }
        }


        if (_active)
        {
            transform.position = (Vector2)Input.mousePosition + _offset;
        }
    }

    protected void SetText(string t)
    {
        _text.text = t;
        _text.supportRichText = true;
        _text.rectTransform.sizeDelta = new Vector2(Mathf.Min(_text.preferredWidth, _maxHorizontalSize - _horizontalSpacing), _text.preferredHeight);
        _image.rectTransform.sizeDelta = new Vector2(Mathf.Min(_text.preferredWidth + _horizontalSpacing, _maxHorizontalSize), _text.preferredHeight + _verticalSpacing);

    }
}
