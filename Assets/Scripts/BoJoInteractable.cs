using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class BoJoInteractable : ShinyInteractable
{
    public UIManager uiManager;
    public bool silent = false;
    
    new IEnumerator Start()
    {
        base.Start();
        
        if (!silent)
        {
            yield return new WaitForSeconds(0.01f);
            Interact(null);
        }
    }

    public override void Interact(ViewConeController viewer)
    {
        uiManager.SetActivePane("BoJoScreen");
        Time.timeScale = 0f;
    }

    public void Close()
    {
        StartCoroutine(CloseInternal());
    }

    // Hacky solution to double-input issue on controllers
    private IEnumerator CloseInternal()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        uiManager.SetActivePane("GameScreen");
        Time.timeScale = 1f;
    }
}
