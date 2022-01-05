using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(CircleCollider2D), typeof(Light2D))]
public class ViewConeController : MonoBehaviour, IFacingListener
{
    private FacingController _facingController;
    private Light2D _viewLight;
    
    private readonly HashSet<Interactable> _itemsInRange = new HashSet<Interactable>();
    private readonly HashSet<Interactable> _itemsInView = new HashSet<Interactable>();
    
    public LayerMask layerMask;
    public float initialFov = 70f;

    private float _fov;
    public float Fov
    {
        get => _fov;
        set
        {
            _fov = value;
            _viewLight.pointLightOuterAngle = value;
        }
    }
    
    private float HFov => Fov / 2f;

    void Start()
    {
        _facingController = GetComponentInParent<FacingController>();
        _viewLight = GetComponent<Light2D>();
        Fov = initialFov;
    }

    public Interactable GetFirstInteractable()
    {
        return _itemsInView.Count <= 0 ? null : _itemsInView.First();
    }

    private void Remove(Interactable item)
    {
        if (_itemsInRange.Contains(item))
        {
            _itemsInRange.Remove(item);
        }

        if (_itemsInView.Contains(item))
        {
            _itemsInView.Remove(item);
            item.ExitView(transform.parent.gameObject);
        }
    }

    private void Clean()
    {
        foreach(var i in _itemsInRange)
        {
            if (i != null) continue;
            Remove(i);
            Clean();
            return;
        }
    }

    private void Update()
    {
        if (_itemsInRange.Count <= 0)
        {
            return;
        }
        
        Clean();
        
        var f = _facingController.Facing;
        var p = transform.position;

        foreach(var i in _itemsInRange)
        {
            var n = i.transform.position - p;
            var nn = n.normalized;
            var a = Vector2.Angle(f, nn);
            if (a <= HFov && !_itemsInView.Contains(i))
            {
                var r = Physics2D.Raycast(p, nn, n.magnitude, layerMask);
                if (r.collider == null)
                {
                    _itemsInView.Add(i);
                    i.EnterView(transform.parent.gameObject);
                }
            } else if (a > HFov && _itemsInView.Contains(i)) {
                _itemsInView.Remove(i);
                i.ExitView(transform.parent.gameObject);
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

        if (item == null) return;

        Remove(item);
    }

    public void OnFacingChange(FacingUtils.Direction old, FacingUtils.Direction updated)
    {
        var a = Vector2.SignedAngle(Vector2.up, FacingUtils.DirectionToVec(updated));
        transform.eulerAngles = new Vector3(0f, 0f, a);
        //Debug.Log(updated);
    }
}
