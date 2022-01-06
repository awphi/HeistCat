using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // Inspector params
    public CanvasGroup[] panes;
    
    private int _activePane = 0;

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

    public void SetActivePane(int x)
    {
        if (_activePane == x) return;
        
        //panes[_activePane].alpha = 0f;
        panes[_activePane].gameObject.SetActive(false);

        //panes[x].alpha = 1f;
        panes[x].gameObject.SetActive(true);
        
        _activePane = x;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void TryAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
