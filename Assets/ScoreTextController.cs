using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreTextController : MonoBehaviour
{
    private TMP_Text _text;
    
    // Start is called before the first frame update
    private void Start()
    {
        _text = GetComponent<TMP_Text>();
    }

    public void OnScoreChanged(int score, int change)
    {
        _text.SetText($"Loot: ${score}.00");
    }
}
