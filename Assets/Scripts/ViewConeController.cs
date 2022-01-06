using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(CircleCollider2D), typeof(Light2D))]
public class ViewConeController : MonoBehaviour, IFacingListener
{
    private FacingController _facingController;
    private Light2D _viewLight;
    private CircleCollider2D _circleCollider;
    
    private readonly HashSet<Interactable> _itemsInRange = new HashSet<Interactable>();
    private readonly HashSet<Interactable> _itemsInView = new HashSet<Interactable>();
    
    public LayerMask layerMask;
    public float initialFov = 70f;
    public float initialRadius = 8;
    public float extraLightRadius = 0.5f;

    private float _fov;
    private float _radius;
    public float Fov
    {
        get => _fov;
        set
        {
            _fov = value;
            _viewLight.pointLightOuterAngle = value;
        }
    }
    
    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            _circleCollider.radius = value;
            _viewLight.pointLightOuterRadius = value + extraLightRadius;
        }
    }
    
    private float HFov => Fov / 2f;

    void Start()
    {
        _facingController = GetComponentInParent<FacingController>();
        _viewLight = GetComponent<Light2D>();
        _circleCollider = GetComponent<CircleCollider2D>();
        
        Fov = initialFov;
        Radius = initialRadius;
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
            item.ExitView(this);
        }
    }

    private void Update()
    {
        if (_itemsInRange.Count <= 0)
        {
            return;
        }
        
        var f = _facingController.Facing;
        var p = transform.position; 

        foreach(var i in _itemsInRange.ToArray())
        {
            if (i == null || !i.enabled)
            {
                Remove(i);
                continue;
            }
            
            var n = i.GetComponent<SpriteRenderer>().bounds.center - p;
            var nn = n.normalized;
            var a = Vector2.Angle(f, nn);
            if (a <= HFov && !_itemsInView.Contains(i))
            {
                var r = Physics2D.Raycast(p, nn, n.magnitude, layerMask);
                if (r.collider == null)
                {
                    _itemsInView.Add(i);
                    i.EnterView(this);
                }
            } else if (a > HFov && _itemsInView.Contains(i)) {
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

        if (item == null) return;

        Remove(item);
    }

    public void OnFacingChange(FacingUtils.Direction old, FacingUtils.Direction updated)
    {
        var a = Vector2.SignedAngle(Vector2.up, FacingUtils.DirectionToVec(updated));
        transform.eulerAngles = new Vector3(0f, 0f, a);
        //Debug.Log(updated);
    }

    public void InteractWithFirst()
    {
        var f = GetFirstInteractable();
        if (f != null)
        {
            f.Interact(this);
        }
    }
}
