using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class ShinyInteractable : Interactable
{
    private SpriteRenderer _spriteRenderer;
    private static readonly int ShaderColor = Shader.PropertyToID("_Color");
    
    private Color _defaultColor;
    public Color highlightColor = Color.yellow;

    protected void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (!_spriteRenderer.material.HasProperty(ShaderColor))
        {
            throw new NotSupportedException("LootInteractable must have shader color property!");
        }

        _defaultColor = _spriteRenderer.material.GetColor(ShaderColor);
    }

    public override void EnterView(GameObject viewer)
    {
        if (_spriteRenderer == null) return;
        _spriteRenderer.material.SetColor(ShaderColor, highlightColor);
    }

    public override void ExitView(GameObject viewer)
    {
        // Can be null when loot has been destroyed
        if (_spriteRenderer == null) return;
        _spriteRenderer.material.SetColor(ShaderColor, _defaultColor);
    }
}
