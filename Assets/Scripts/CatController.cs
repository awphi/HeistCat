using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CatController : MonoBehaviour
{
    public float speed = 5;
    public int nerveRechargeTime = 3;
    public int maxNerve = 250;

    public TMP_Text nerveText;
    
    private Rigidbody2D _rb2d;
    private Animator _animator;
    private ParticleSystem _sleepParticles;
    private ViewConeController _viewConeController;
    [HideInInspector]
    public SpeechController speechController;
    private Light2D[] _lights;

    private int _nerve;
    private bool _nerveRecharging;
    private Vector2 _input;
    private bool _waitingForNerveRecharge = true;
    private bool _sleepingDown = false;
    
    private static readonly int AnimSleeping = Animator.StringToHash("sleeping");
    
    public bool IsSleeping => _animator.GetBool(AnimSleeping);

    // Start is called before the first frame update
    private void Start()
    {
        _nerve = maxNerve;
        _rb2d = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        speechController = GetComponentInChildren<SpeechController>();
        _viewConeController = GetComponentInChildren<ViewConeController>();
        _lights = GetComponentsInChildren<Light2D>();
        _sleepParticles = transform.Find("SleepParticles").GetComponent<ParticleSystem>();
    }

    public void OnScoreChanged(int score, int change)
    {
        speechController.Say($"+${change}.00", Color.green);
    }

    private IEnumerator Recharge () {
        yield return new WaitForSeconds (nerveRechargeTime);
        _nerveRecharging = true;
    }

    private void SetLights(bool on)
    {
        foreach (var i in _lights)
        {
            i.enabled = on;
        }
    }

    private void SetSleep(bool s)
    {
        _animator.SetBool(AnimSleeping, s);
        if (s && !_sleepParticles.isPlaying) {
            _sleepParticles.Play();
            SetLights(false);
        } else if(!s && _sleepParticles.isPlaying) {
            _sleepParticles.Stop();
            _sleepParticles.Clear();
            SetLights(true);
        }
    }

    private void UpdateNerve()
    {
        if (_nerveRecharging)
        {
            if (_nerve >= maxNerve)
            {
                _nerveRecharging = false;
            } else {
                _nerve += 1;
            }
        } else {
            if (_nerve > 0 && _sleepingDown)
            {
                SetSleep(true);
                _nerve -= 1;

                _waitingForNerveRecharge = true;
                StopCoroutine(nameof(Recharge));
            }
            else
            {
                SetSleep(false);
                if (_waitingForNerveRecharge)
                {
                    _waitingForNerveRecharge = false;
                    StartCoroutine(nameof(Recharge));
                }
            }
        }

        var f = (float) _nerve / maxNerve * 100f;
        nerveText.text = "Nerve: " + $"{f:F1}";;
    }
    
    public void OnMove(InputValue value)
    {
        _input = value.Get<Vector2>();
    }

    public void OnInteract(InputValue value)
    {
         _viewConeController.InteractWithFirst();
    }

    public void OnSleep(InputValue value)
    {
        _sleepingDown = value.Get<float>() > 0f;
    }

    private void FixedUpdate()
    {
        UpdateNerve();

        var p = _rb2d.velocity;
        if (!_animator.GetBool(AnimSleeping))
        {
            _rb2d.velocity = _input * speed;
        }
        else
        {
            _rb2d.velocity = Vector2.zero;
        }
    }
}
