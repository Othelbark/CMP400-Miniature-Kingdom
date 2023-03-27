using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour
{
    [SerializeField]
    protected Image _image;
    [SerializeField]
    protected Text _text;

    [SerializeField]
    protected Vector2 _offset;

    [SerializeField]
    protected float _tooltipLag = 0.5f;
    protected float _hoverTime = 0;
    protected TooltipedObject _hoveredObject;
    protected Vector3 _hoverPos;

    [SerializeField]
    protected float _verticalSpacing = 20.0f;
    [SerializeField]
    protected float _horizontalSpacing = 20.0f;
    [SerializeField]
    protected float _maxHorizontalSize = 200.0f;

    protected bool _active = false;


    public ContactFilter2D tooltipableFilter;
    public ContactFilter2D tooltipableFilterUI;

    GraphicRaycaster _Raycaster;
    PointerEventData _PointerEventData;
    EventSystem _EventSystem;

    // Start is called before the first frame update
    void Start()
    {
        _text.gameObject.SetActive(false);
        _image.gameObject.SetActive(false);

        
        _Raycaster = GetComponentInParent<GraphicRaycaster>();
        _EventSystem = GetComponent<EventSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        TooltipedObject tooltipedObject = null;

        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        //Set up the new Pointer Event
        _PointerEventData = new PointerEventData(_EventSystem);
        _PointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        _Raycaster.Raycast(_PointerEventData, results);

        if (results.Count > 0)
        {
            tooltipedObject = results[0].gameObject.GetComponentInParent<TooltipedObject>();
        }


        if (tooltipedObject != null)
        {
            TooltipHoverCheck(tooltipedObject);
        }
        else if (Physics2D.Raycast(mousePos2D, Vector2.zero, tooltipableFilter, hits) > 0)
        {
            if (tooltipedObject = hits[0].transform.gameObject.GetComponent<TooltipedObject>())
            {
                TooltipHoverCheck(tooltipedObject);
            }
            else
            {
                DeactivateTooltip();
                Debug.LogWarning("No tooltip on object in Tooltipable filter");
            }
        }
        else
        {
            DeactivateTooltip();
        }


        if (_active)
        {
            transform.position = (Vector2)Input.mousePosition + _offset;

            if (transform.position.y - _image.rectTransform.sizeDelta.y < 0)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + _image.rectTransform.sizeDelta.y - (_offset.y * 2), transform.position.z);
            }

            if (transform.position.x + _image.rectTransform.sizeDelta.x > Screen.width)
            {
                transform.position = new Vector3(transform.position.x - _image.rectTransform.sizeDelta.x - (_offset.x * 2), transform.position.y, transform.position.z);
            }
        }
    }

    protected void TooltipHoverCheck(TooltipedObject tooltipedObject)
    {
        // Instantly tooltip agents
        if (tooltipedObject is Agent)
        {
            ActivateTooltip(tooltipedObject.GetText());
        }

        if (tooltipedObject == _hoveredObject && _hoverPos == Input.mousePosition)
        {
            _hoverTime += Time.deltaTime;
            if (_hoverTime >= _tooltipLag)
            {
                ActivateTooltip(tooltipedObject.GetText());
            }
        }
        else
        {
            if (tooltipedObject != _hoveredObject)
                DeactivateTooltip();

            _hoverTime = 0;
            _hoveredObject = tooltipedObject;
            _hoverPos = Input.mousePosition;
        }
    }
    protected void ActivateTooltip(string text)
    {
        SetText(text);
        if (!_active)
        {
            _active = true;
            _text.gameObject.SetActive(true);
            _image.gameObject.SetActive(true);
        }
    }
    protected void DeactivateTooltip()
    {
        if (_active)
        {
            _hoverTime = 0;
            _hoveredObject = null;

            _active = false;
            _text.gameObject.SetActive(false);
            _image.gameObject.SetActive(false);
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


