using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class LootInteractable : Interactable
{
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    private ViewConeController _lastViewer;
    
    private static readonly int ShaderColor = Shader.PropertyToID("_Color");
    private Color _defaultColor;

    public Color highlightColor = Color.yellow;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        
        if (!_spriteRenderer.material.HasProperty(ShaderColor))
        {
            throw new NotSupportedException("LootInteractable must have shader color property!");
        }

        _defaultColor = _spriteRenderer.material.GetColor(ShaderColor);
    }

    public override void EnterView(ViewConeController viewer)
    {
        _spriteRenderer.material.SetColor(ShaderColor, highlightColor);
    }

    public override void ExitView(ViewConeController viewer)
    {
        _spriteRenderer.material.SetColor(ShaderColor, _defaultColor);
    }

    public override void Interact(ViewConeController viewer)
    {
        _lastViewer = viewer;
        _animator.Play("LootPickupAnimation");
    }

    public void Pickup()
    {
        _lastViewer.Remove(this);
        Destroy(gameObject);
        // TODO add points
    }
}
