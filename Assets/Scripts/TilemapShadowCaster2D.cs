using UnityEngine;

/// <summary>
/// Generates ShadowCaster2Ds for every continuous block of a tilemap on Awake, applying some settings.
/// </summary>
public class TilemapShadowCaster2D : MonoBehaviour
{
    [SerializeField]
    protected CompositeCollider2D tilemapCollider;
 
    [SerializeField]
    protected bool selfShadows = true;
 
    protected virtual void Reset()
    {
        tilemapCollider = GetComponent<CompositeCollider2D>();
    }
 
    protected virtual void Awake()
    {
        ShadowCaster2DGenerator.GenerateTilemapShadowCasters(tilemapCollider, selfShadows);
    }
}