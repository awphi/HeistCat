using System;
using UnityEngine;
using UnityEngine.Serialization;

public class FacingController : MonoBehaviour
{
    public FacingUtils.Direction initialDirection = FacingUtils.Direction.Right;
    
    private IFacingListener[] _listeners;

    private void Awake()
    {
        _listeners = GetComponentsInChildren<IFacingListener>();
    }

    public void Start()
    {
        SetFacing(initialDirection);
    }
    
    public FacingUtils.Direction Direction { get; private set; }

    public Vector2 Facing => FacingUtils.DirectionToVec(Direction);

    public void SetFacing(FacingUtils.Direction f)
    {
        var old = Direction;
        Direction = f;

        foreach (var l in _listeners)
        {
            l.OnFacingChange(old, f);
        }
    }
}
