using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class CatController : MonoBehaviour
{
    public float speed = 6;
    public int nerveRechargeTime = 5;
    public int maxNerve = 1000;

    public Text nerveText;
    
    private Rigidbody2D _rb2d;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private ParticleSystem _sleepParticles;
    
    private int _nerve;
    private bool _nerveRecharging = false;
    private bool _waitingForNerveRecharge = true;

    private static readonly int MovingX = Animator.StringToHash("moving_x");
    private static readonly int VelY = Animator.StringToHash("vel_y");
    private static readonly int Sleeping = Animator.StringToHash("sleeping");

    // Start is called before the first frame update
    private void Start()
    {
        _nerve = maxNerve;
        _rb2d = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _sleepParticles = transform.Find("SleepParticles").GetComponent<ParticleSystem>();
        _sleepParticles.Stop();
    }
    
    private IEnumerator Recharge () {
        yield return new WaitForSeconds (nerveRechargeTime);
        _nerveRecharging = true;
    }

    private void SetSleep(bool s)
    {
        _animator.SetBool(Sleeping, s);
        if (s && !_sleepParticles.isPlaying) {
            _sleepParticles.Play();   
        } else if(!s && _sleepParticles.isPlaying) {
            _sleepParticles.Stop();
            _sleepParticles.Clear();
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
        UpdateNerve();
        
        var vel = new Vector2(0f, 0f);
        if (!_animator.GetBool(Sleeping))
        {
            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");
        
            if (math.abs(h) > 0.1)
            {
                vel.x = math.sign(h) * speed;
                transform.eulerAngles = new Vector3(0f, vel.x > 0f ? 0f : 180f, 0f);
            }
        
            if (math.abs(v) > 0.1)
            {
                vel.y = math.sign(v) * speed;
            }

        }

        _rb2d.velocity = vel;
        _animator.SetBool(MovingX, vel.x != 0);
        _animator.SetFloat(VelY, vel.y);
    }
}
