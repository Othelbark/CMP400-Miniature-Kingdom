using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionDisplayScript : MonoBehaviour
{
    public ContactFilter2D cantOverlap;

    protected List<Collider2D> _overlapping = new List<Collider2D>();

    protected SpriteRenderer _renderer;

    [SerializeField]
    protected Color _cantBuildColour;
    [SerializeField]
    protected Color _canBuildColour;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Collider2D>().OverlapCollider(cantOverlap, _overlapping);

        if (_overlapping.Count > 0)
        {
            _renderer.color = _cantBuildColour;
        }
        else
        {
            _renderer.color = _canBuildColour;
        }
    }


    public bool GetOverlapping()
    {

        if (_overlapping.Count > 0)
        {
            return true;
        }
        return false;
    }
}
