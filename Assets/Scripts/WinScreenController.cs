using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WinScreenController : MonoBehaviour
{
    public CatController catController;

    private ScoreManager _scoreManager;
    private TMP_Text _pilferedText;
    private TMP_Text _bestText;
    
    private void Awake()
    {
        _scoreManager = ScoreManager.Get();
        _pilferedText = transform.Find("TextContainer").Find("LootText").GetComponent<TMP_Text>();
        _bestText = transform.Find("TextContainer").Find("PrevBestText").GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        var now = DateTime.Now.ToBinary().ToString();
        var best = PlayerPrefs.GetFloat("best", 0f);
        var bestDate = PlayerPrefs.GetString("bestDate", now);

        if (_scoreManager.Score > best)
        {
            best = _scoreManager.Score;
            bestDate = now;
            PlayerPrefs.SetFloat("best", best);
            PlayerPrefs.SetString("bestDate", now);
        }
        
        
        var date = DateTime.FromBinary(Convert.ToInt64(bestDate));
        
        _pilferedText.text = $"${_scoreManager.Score}.00";
        _bestText.text = "best: $" + $"{best:0.00}" + " @ " + date.ToShortDateString();
        
        Time.timeScale = 0f;
    }
}
