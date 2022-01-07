using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
        
    public UnityEvent<int, int> scoreChanged;
    private int _score = 0;
    public int Score
    {
        get => _score;
        set
        {
            if (_score == value) return;
            var old = _score;
            _score = value;
            scoreChanged.Invoke(_score, _score - old);
        }
    }

    public static ScoreManager Get()
    {
        Debug.Assert(Camera.main != null, "Camera.main != null");
        return Camera.main.GetComponentInParent<ScoreManager>();
    }
}
