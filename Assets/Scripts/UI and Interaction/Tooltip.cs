using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    protected RectTransform _transform;
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

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            _transform = GetComponent<RectTransform>();
        }
        catch
        {
            Debug.LogError("Tooltip has no rect transform");
        }

        _text.rectTransform.sizeDelta = new Vector2(Mathf.Min(_text.preferredWidth, _maxHorizontalSize - _horizontalSpacing), _text.preferredHeight);
        _transform.sizeDelta = new Vector2(Mathf.Min(_text.preferredWidth + _horizontalSpacing, _maxHorizontalSize), _text.preferredHeight + _verticalSpacing);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = (Vector2)Input.mousePosition + _offset;

    }

    public void SetText(string t)
    {
        _text.text = t;
        _text.rectTransform.sizeDelta = new Vector2(Mathf.Min(_text.preferredWidth, _maxHorizontalSize - _horizontalSpacing), _text.preferredHeight);
        _transform.sizeDelta = new Vector2(Mathf.Min(_text.preferredWidth + _horizontalSpacing, _maxHorizontalSize), _text.preferredHeight + _verticalSpacing);

    }
}
