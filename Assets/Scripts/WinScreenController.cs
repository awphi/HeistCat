using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinScreenController : MonoBehaviour
{
    public CatController catController;
    private TMP_Text _pilferedText;
    private TMP_Text _bestText;
    
    private void Awake()
    {
        _pilferedText = transform.Find("TextContainer").Find("LootText").GetComponent<TMP_Text>();
        _bestText = transform.Find("TextContainer").Find("PrevBestText").GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        var now = DateTime.Now.ToBinary().ToString();
        var best = PlayerPrefs.GetFloat("best", 0f);
        var bestDate = PlayerPrefs.GetString("bestDate", now);
        
        if (catController.Score > best)
        {
            best = catController.Score;
            bestDate = now;
            PlayerPrefs.SetFloat("best", best);
            PlayerPrefs.SetString("bestDate", now);
        }
        
        var date = DateTime.FromBinary(Convert.ToInt64(bestDate));
        
        _pilferedText.text = "$" + $"{catController.Score:0.00}";
        _bestText.text = "best: " + $"{best:0.00}" + " @ " + date.ToShortDateString();
        
        Time.timeScale = 0f;
    }
}
