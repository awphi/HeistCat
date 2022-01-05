using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoJoInteractable : ShinyInteractable
{
    public GameObject dialogue;

    void Start()
    {
        base.Start();
        Interact(null);
    }

    public override void Interact(GameObject viewer)
    {
        dialogue.SetActive(true);
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Sleep"))
        {
            Close();
        }
    }

    private void Close()
    {
        dialogue.SetActive(false);
        Time.timeScale = 1f;
    }
}
