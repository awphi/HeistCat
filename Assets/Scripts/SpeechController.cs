using System;
using TMPro;
using UnityEngine;

public class SpeechController : MonoBehaviour
{
    private TMP_Text _text;
    private Animator _animator;
    
    public static readonly int AnimDoFloat = Animator.StringToHash("DoFloat");
    public static readonly int AnimDoFade = Animator.StringToHash("DoFade");
    public static readonly int AnimDoGrow = Animator.StringToHash("DoGrow");
    private static readonly int AnimSpeed = Animator.StringToHash("speed");

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
        _animator = GetComponent<Animator>();
    }
    

    public void Say(string str, Color color, int anim=-1, float speed=1.0f, int size=4)
    {
        if (anim == -1)
        {
            anim = AnimDoFloat;
        }
        
        _text.SetText(str);
        _text.fontSize = size;
        _text.color = color;
        _animator.SetFloat(AnimSpeed, speed);
        _animator.SetTrigger(anim);
    }
}
