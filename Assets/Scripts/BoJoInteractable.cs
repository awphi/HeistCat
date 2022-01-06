using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoJoInteractable : ShinyInteractable
{
    public UIManager uiManager;

    new IEnumerator Start()
    {
        base.Start();
        yield return new WaitForSeconds(0.01f);
        Interact(null);
    }

    public override void Interact(ViewConeController viewer)
    {
        if (uiManager.SetActivePane("BoJoScreen"))
        {
            Time.timeScale = 0f;
        }
    }

    public void Close()
    {
        uiManager.SetActivePane("GameScreen");
        Time.timeScale = 1f;
    }
}
