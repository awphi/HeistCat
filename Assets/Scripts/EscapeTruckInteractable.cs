using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class EscapeTruckInteractable : ShinyInteractable
{
    public UIManager uiManager;
    private ScoreManager _scoreManager;

    new void Start()
    {
        base.Start();
        _scoreManager = ScoreManager.Get();
    }
    
    public override void Interact(ViewConeController viewer)
    {
        var cat = viewer.GetComponentInParent<CatController>();
        if (cat == null) return;

        if (_scoreManager.Score <= 0)
        {
            cat.speechController.Say("I should probably grab some loot before trying to leave!", Color.white, anim: SpeechController.AnimDoFloat, speed: 0.2f, size: 6);
        }
        else
        {
            uiManager.SetActivePane("WinScreen");
        }
    }
}
