using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class CatInteractable : Interactable
{
    public GameObject bloodSplatter;
    public MainCameraController mainCameraController;
    public UIManager uiManager;

    private bool _dead = false;

    [HideInInspector]
    public CatController controller;
    
    private void Awake()
    {
        controller = GetComponent<CatController>();
    }

    public override void EnterView(ViewConeController viewer)
    {
        // On EnterView make the guard who spotted cat chase it
        var guardAi = viewer.GetComponentInParent<GuardAI>();
        if (guardAi == null) return;
        
        if (viewer.name == "KillerViewCone")
        {
            Kill(guardAi);
        }
        else
        {
            guardAi.OnSeeCat(this);
        }
    }

    public override void ExitView(ViewConeController viewer)
    {
        // On ExitView set last seen marker
        var guardAi = viewer.GetComponentInParent<GuardAI>();
        if (guardAi == null) return;
        
        guardAi.OnUnseeCat(this);
    }

    public override void Interact(ViewConeController viewer) { }

    private void Kill(GuardAI killer)
    {
        if (_dead)
        {
            return;
        }
        
        var str = " *oink* Got you!";
        if (controller.IsSleeping)
        {
            str = "Can't fool me! *oink*";
        }

        if (killer != null)
        {
            killer.speechController.Say(str, Color.red, size: 6, anim: SpeechController.AnimDoFade, speed: 0.3f);
        }

        var t = transform;
        Instantiate(bloodSplatter, t.position, t.rotation);
        _dead = true;
        gameObject.SetActive(false);
        mainCameraController.StartCoroutine(mainCameraController.Shake(0.2f, 0.3f));
        uiManager.SetActivePane("DeathScreen");
    }
}
