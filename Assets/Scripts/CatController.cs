using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public class CatController : MonoBehaviour
{
    public float speed = 5;
    public int nerveRechargeTime = 3;
    public int maxNerve = 250;
    public float Score { get; private set; }


    public TMP_Text nerveText;
    public TMP_Text scoreText;
    
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
        
        SetScore(0);
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

    public void SetScore(float a)
    {
        Score = a;
        scoreText.SetText("Loot: $" + $"{Score:0.00}");
    }

    public void AddScore(float amount)
    {
        speechController.Say("+$" + $"{amount:0.00}", Color.green);
        SetScore(Score + amount);
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
            if (_nerve > 0 && Input.GetButton("Sleep"))
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

    private void Update()
    {
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
    
        _input.x = math.abs(h) > 0.1 ? math.sign(h) : 0f;
        _input.y = math.abs(v) > 0.1 ? math.sign(v) : 0f;

        var e = Input.GetButtonUp("Interact");
        if (e)
        {
            var f = _viewConeController.GetFirstInteractable();
            if (f != null)
            {
                f.Interact(gameObject);
            }
        }
        
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
