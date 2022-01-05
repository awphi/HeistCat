using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoJoInteractable : ShinyInteractable
{
    public UIManager uiManager;
    
    new void Start()
    {
        base.Start();
        Interact(null);
    }

    public override void Interact(GameObject viewer)
    {
        uiManager.SetActivePane("BoJoScreen");
        Time.timeScale = 0f;
    }

    public void Close()
    {
        uiManager.SetActivePane("GameScreen");
        Time.timeScale = 1f;
    }
}
