using System;
using System.Collections;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class LootInteractable : ShinyInteractable
{
    private Animator _animator;
    private AudioSource _audio;
    private BoxCollider2D _collider;
    private ScoreManager _scoreManager;

    public LootData lootData;

    private ViewConeController _lastViewer;
    private static readonly int AnimPickup = Animator.StringToHash("Pickup");
    private static readonly int AnimSpawn = Animator.StringToHash("Spawn");

    private new void Start()
    {
        base.Start();
        _animator = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
        _collider = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _scoreManager = ScoreManager.Get();
    }
    
    public override void Interact(ViewConeController viewer)
    {
        if (_lastViewer != null)
        {
            return;
        }
        
        _lastViewer = viewer;
        var cat = _lastViewer.GetComponentInParent<CatController>();
        if (cat != null)
        {
            var loot = lootData.RollRandomLoot();
            _scoreManager.Score += loot;
        }
        _animator.SetTrigger(AnimPickup);
        _audio.Play();
    }

    private void SetActive(bool b)
    {
        enabled = b;
        _collider.enabled = b;
        _spriteRenderer.enabled = b;
    }
    
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(Random.Range(lootData.minRespawnTime, lootData.maxRespawnTime + 1));
        SetActive(true);
        _lastViewer = null;
        _animator.SetTrigger(AnimSpawn);
    }

    public void Pickup()
    {
        // TODO move coroutine call to another monobehaviour or only disable this script + sprite renderer
        StartCoroutine(Respawn());
        SetActive(false);
    }
}
