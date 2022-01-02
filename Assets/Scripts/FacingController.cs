using System;
using UnityEngine;
using UnityEngine.Serialization;

public class FacingController : MonoBehaviour
{
    private static readonly int AnimFacing = Animator.StringToHash("facing");
    private Animator _animator;
    private FacingUtils.Direction _direction;
    
    public FacingUtils.Direction initialDirection = FacingUtils.Direction.Right;

    public void Start()
    {
        _animator = GetComponent<Animator>();
        SetFacing(initialDirection);
    }

    public Vector2 Facing => FacingUtils.DirectionToVec(_direction);
    
    public void SetFacing(FacingUtils.Direction f)
    {
        _direction = f;
        // Will flip the sprite if heading left
        transform.eulerAngles = _direction == FacingUtils.Direction.Left ? new Vector3(0f, 180f, 0f) : new Vector3(0f, 0f, 0f);
        if (_animator != null)
        {
            _animator.SetInteger(AnimFacing, (int) _direction);
        }
    }
}
