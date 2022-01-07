using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class ShinyInteractable : Interactable
{
    protected SpriteRenderer _spriteRenderer;
    private static readonly int ShaderColor = Shader.PropertyToID("_Color");
    
    private Color _defaultColor;
    private float _defaultIntensity;
    
    public Color highlightColor = Color.yellow;
    public float highlightIntensityMultiplier = 2f;
    
    private static readonly int ShaderIntensity = Shader.PropertyToID("_Intensity");

    protected void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (!_spriteRenderer.material.HasProperty(ShaderColor))
        {
            throw new NotSupportedException("LootInteractable must have shader color property!");
        }

        _defaultColor = _spriteRenderer.material.GetColor(ShaderColor);
        _defaultIntensity = _spriteRenderer.material.GetFloat(ShaderIntensity);
    }

    public override void EnterView(ViewConeController viewer)
    {
        if (_spriteRenderer == null) return;
        _spriteRenderer.material.SetColor(ShaderColor, highlightColor);
        _spriteRenderer.material.SetFloat(ShaderIntensity, _defaultIntensity * highlightIntensityMultiplier);
    }

    public override void ExitView(ViewConeController viewer)
    {
        // Can be null when loot has been destroyed
        if (_spriteRenderer == null) return;
        _spriteRenderer.material.SetColor(ShaderColor, _defaultColor);
        _spriteRenderer.material.SetFloat(ShaderIntensity, _defaultIntensity);
    }
}
