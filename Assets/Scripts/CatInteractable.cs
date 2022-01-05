using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class CatInteractable : Interactable
{
    public GameObject bloodSplatter;

    public CatController controller;
    
    private void Awake()
    {
        controller = GetComponent<CatController>();
    }

    public override void EnterView(GameObject viewer)
    {
        // On EnterView make the guard who spotted cat chase it
        var guardAi = viewer.GetComponent<GuardAI>();
        if (guardAi == null) return;
        
        guardAi.OnSeeCat(this);
    }

    public override void ExitView(GameObject viewer)
    {
        // On ExitView set last seen marker
        var guardAi = viewer.GetComponent<GuardAI>();
        if (guardAi == null) return;
        
        guardAi.OnUnseeCat(this);
    }

    public override void Interact(GameObject viewer)
    {
        var guardAi = viewer.GetComponent<GuardAI>();
        if (guardAi == null) return;

        var t = transform;
        var d = (t.position - viewer.transform.position).magnitude;

        if (!controller.IsSleeping)
        {
            guardAi.ChaseCat(this);
        }
        
        // If close enough - kill
        if (d <= guardAi.KillRange)
        {
            var str = " *oink* Got you!";
            if (controller.IsSleeping)
            {
                str = "Can't fool me! *oink*";
            }
            
            guardAi.speechController.Say(str, Color.red, size: 6, anim: SpeechController.AnimDoFade, speed:0.3f);

            Instantiate(bloodSplatter, t.position, t.rotation);
            Destroy(gameObject);
        }

    }
}
