using System;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(FacingController))]
public class FourWayFacingRenderer : MonoBehaviour, IFacingListener
{
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rb2d;
    private AIPath _aiPath;
    private FacingController _facingController;
    
    private static readonly int AnimFacing = Animator.StringToHash("facing");
    private static readonly int AnimMoving = Animator.StringToHash("moving");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _facingController = GetComponent<FacingController>();
        
        _rb2d = GetComponent<Rigidbody2D>();
        _aiPath = GetComponent<AIPath>();
    }

    private void UpdateFacingWithVelocity(Vector2 velocity)
    {
        // Old: Check if facing is already correct
        //if (!(Vector2.Dot(velocity, _facingController.Facing) <= 0)) return;

        var xAbs = math.abs(velocity.x);
        var yAbs = math.abs(velocity.y);

        // Check if vel is practically zero, ignored to prevent jitters from small bounces against objects
        if (xAbs <= 0.001f && yAbs < 0.001f)
        {
            return;
        }
        
        // TODO edit this so its a scalar-based priority not x-first
        if (xAbs > yAbs)
        {
            _facingController.SetFacing(velocity.x > 0 ? FacingUtils.Direction.Right : FacingUtils.Direction.Left);
        } else if (yAbs > xAbs)
        {
            _facingController.SetFacing(velocity.y > 0 ? FacingUtils.Direction.Up : FacingUtils.Direction.Down);
        }
    }

    private void Update()
    {
        var velocity = Vector2.zero;
        
        
        if (_aiPath != null)
        {
            velocity = _aiPath.desiredVelocity;
        } else if(_rb2d != null)
        {
            velocity = _rb2d.velocity;
        }


        UpdateFacingWithVelocity(velocity);
        _animator.SetBool(AnimMoving, math.abs(velocity.magnitude) > 0);
    }

    public void OnFacingChange(FacingUtils.Direction old, FacingUtils.Direction updated)
    {
        //transform.eulerAngles = _direction == FacingUtils.Direction.Left ? new Vector3(0f, 180f, 0f) : new Vector3(0f, 0f, 0f);
        _spriteRenderer.flipX = updated == FacingUtils.Direction.Left;
        _animator.SetInteger(AnimFacing, (int) updated);
    }
}
