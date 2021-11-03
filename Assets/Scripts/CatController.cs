using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class CatController : MonoBehaviour
{
    public float speed = 5;
    public int nerveRechargeTime = 3;
    public int maxNerve = 250;

    public Text nerveText;
    
    private Rigidbody2D _rb2d;
    private Animator _animator;
    private ParticleSystem _sleepParticles;

    private int _nerve;
    private bool _nerveRecharging;
    private Vector2 _input;
    private bool _waitingForNerveRecharge = true;
    private int _facing;

    private Vector2 Facing => Utils.GetFacing(_facing);

    private static readonly int AnimVelX = Animator.StringToHash("vel_x");
    private static readonly int AnimVelY = Animator.StringToHash("vel_y");
    private static readonly int AnimSleeping = Animator.StringToHash("sleeping");
    private static readonly int AnimFacing = Animator.StringToHash("facing");

    // Start is called before the first frame update
    private void Start()
    {
        _nerve = maxNerve;
        _rb2d = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _sleepParticles = transform.Find("SleepParticles").GetComponent<ParticleSystem>();
    }
    
    private IEnumerator Recharge () {
        yield return new WaitForSeconds (nerveRechargeTime);
        _nerveRecharging = true;
    }

    private void SetSleep(bool s)
    {
        _animator.SetBool(AnimSleeping, s);
        if (s && !_sleepParticles.isPlaying) {
            _sleepParticles.Play();   
        } else if(!s && _sleepParticles.isPlaying) {
            _sleepParticles.Stop();
            _sleepParticles.Clear();
        }
    }

    private void SetFacing(int f)
    {
        _facing = f;
        // Will flip the sprite if heading left
        transform.eulerAngles = _facing == 3 ? new Vector3(0f, 180f, 0f) : new Vector3(0f, 0f, 0f);
        _animator.SetInteger(AnimFacing, _facing);
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
            if (_nerve > 0 && Input.GetButton("Fire1"))
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
        
        if (Vector2.Dot(_rb2d.velocity, Facing) <= 0)
        {
            if (_rb2d.velocity.x != 0)
            {
                SetFacing(_rb2d.velocity.x > 0 ? 1 : 3);
            } else if (_rb2d.velocity.y != 0)
            {
                SetFacing(_rb2d.velocity.y > 0 ? 0 : 2);
            }
        }
        
        _animator.SetFloat(AnimVelX, _rb2d.velocity.x);
        _animator.SetFloat(AnimVelY, _rb2d.velocity.y);
        //transform.eulerAngles = new Vector3(0f, _rb2d.velocity.x > 0f ? 0f : 180f, 0f);
    }
}
