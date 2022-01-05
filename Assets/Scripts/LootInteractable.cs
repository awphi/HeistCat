using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class LootInteractable : ShinyInteractable
{
    private Animator _animator;
    private AudioSource _audio;

    public int value = 100;

    private GameObject _lastViewer;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
    }

    public override void Interact(GameObject viewer)
    {
        if (_lastViewer != null)
        {
            return;
        }
        
        _lastViewer = viewer;
        var cat = _lastViewer.GetComponentInParent<CatController>();
        if (cat != null)
        {
            cat.AddScore(value);
        }
        _animator.Play("LootPickupAnimation");
        _audio.Play();
    }

    public void Pickup()
    {
        //Destroy(coll);
        Destroy(gameObject);
    }
}
