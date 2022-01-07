using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Inspector params
    public CanvasGroup[] panes;
    
    private int _activePane = -1;

    private void Start()
    {
        foreach (var p in panes)
        {
            p.gameObject.SetActive(false);
        }

        SetActivePane(0);
    }

    public bool SetActivePane(string name)
    {
        var c = 0;
        foreach (var p in panes)
        {
            if (p.gameObject.name == name)
            {
                SetActivePane(c);
                return true;
            }

            c++;
        }

        return false;
    }

    private IEnumerator Select(Selectable sel)
    {
        yield return new WaitForSecondsRealtime(0.1f);
        sel.Select();
    }

    public void SetActivePane(int x)
    {
        if (_activePane == x) return;
        
        //panes[_activePane].alpha = 0f;
        if (_activePane >= 0 && _activePane < panes.Length)
        {
            panes[_activePane].gameObject.SetActive(false);
        }

        //panes[x].alpha = 1f;
        panes[x].gameObject.SetActive(true);

        var btn = panes[x].gameObject.GetComponentInChildren<Button>();
        if (btn != null)
        {
            StartCoroutine(Select(btn));
        }
        
        _activePane = x;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void TryAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
