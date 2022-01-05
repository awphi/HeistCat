using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeTruckInteractable : ShinyInteractable
{
    public override void Interact(GameObject viewer)
    {
        var cat = viewer.GetComponent<CatController>();
        if (cat == null) return;

        if (cat.Score >= 0)
        {
            cat.speechController.Say("I should probably grab some loot before trying to leave!", Color.white, anim: SpeechController.AnimDoFloat, speed: 0.2f, size: 6);
        }
    }
}
