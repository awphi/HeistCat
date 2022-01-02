using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ViewConeController : MonoBehaviour
{
    private CircleCollider2D _circleCollider;
    private FacingController _facingController;
    
    private readonly HashSet<Interactable> _itemsInRange = new HashSet<Interactable>();
    private readonly HashSet<Interactable> _itemsInView = new HashSet<Interactable>();

    public LayerMask layerMask;

    private float _hFov;

    public float fov = 70f;

    void Start()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
        _facingController = GetComponentInParent<FacingController>();
        _hFov = fov / 2;
    }

    public Interactable GetFirstInteractable()
    {
        return _itemsInView.Count <= 0 ? null : _itemsInView.First();
    }

    public void Remove(Interactable i)
    {
        _itemsInRange.Remove(i);
        _itemsInView.Remove(i);
    }

    private void Update()
    {
        if (_itemsInRange.Count <= 0)
        {
            return;
        }
        
        var f = _facingController.Facing;
        var p = transform.position;
        
        foreach(var i in _itemsInRange)
        {
            var n = i.transform.position - p;
            var nn = n.normalized;
            var a = Vector2.Angle(f, nn);
            if (a <= _hFov && !_itemsInView.Contains(i))
            {
                var r = Physics2D.Raycast(p, nn, n.magnitude, layerMask);
                if (r.collider == null)
                {
                    _itemsInView.Add(i);
                    i.EnterView(this);
                }
            } else if (a > _hFov && _itemsInView.Contains(i)) {
                _itemsInView.Remove(i);
                i.ExitView(this);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var item = other.gameObject.GetComponent<Interactable>();

        if (item == null || _itemsInRange.Contains(item)) return;
        
        _itemsInRange.Add(item);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        var item = other.gameObject.GetComponent<Interactable>();

        if (item == null || !_itemsInRange.Contains(item)) return;
        
        _itemsInRange.Remove(item);
    }
}
