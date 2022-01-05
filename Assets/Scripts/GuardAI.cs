using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Serialization;

public class GuardAI : MonoBehaviour
{
    // Comp
    private AIDestinationSetter _destinationSetter;
    private AIPath _aiPath;
    public SpeechController speechController;
    private AudioSource _audio;
    private FacingController _facingController;
    private Light2D _killLight;

    // Params
    public Transform catMarker;
    public GameObject path;
    public float patrolDelay = 0f;
    public float peripheralTime = 0.5f;
    public float lastSeenDelay = 0.5f;
    public float initialKillRange = 3.5f;

    public bool IsChasingCat { get; private set; }

    // Backing Props
    private bool _patrolPaused = false;
    private float _killRange;
    private Transform[] _nodes;
    private float _switchTime = float.PositiveInfinity;
    private int _index = 0;
    private CatInteractable _trackingCat;
    private Coroutine _stopTrackingCoroutine;

    public float KillRange
    {
        get => _killRange;
        set
        {
            _killRange = value;
            _killLight.pointLightOuterRadius = value;
        }
    }

    private void Awake()
    {
        speechController = GetComponentInChildren<SpeechController>();
        _aiPath = GetComponent<AIPath>();
        _audio = GetComponent<AudioSource>();
        _facingController = GetComponent<FacingController>();
        _destinationSetter = GetComponent<AIDestinationSetter>();
        _killLight = transform.GetComponentInChildren<ViewConeController>().transform.Find("KillLight").GetComponent<Light2D>();

        KillRange = initialKillRange;

        _nodes = new Transform[path.transform.childCount];
        
        for (var i = 0; i < path.transform.childCount; i++)
        {
            _nodes[i] = path.transform.GetChild(i).transform;
        }
    }

    public void ChaseCat(CatInteractable cat)
    {
        if (!IsChasingCat)
        {
            IsChasingCat = true;
            SetPatrolPaused(true);
            _destinationSetter.target = cat.transform;
            _aiPath.autoRepath.mode = AutoRepathPolicy.Mode.Dynamic;
            _aiPath.SearchPath();
            speechController.Say("!", Color.red, size: 12, anim: SpeechController.AnimDoGrow);
            _audio.Play();
        }
    }

    public void OnSeeCat(CatInteractable cat)
    {
        if (_trackingCat == null)
        {
            if (_stopTrackingCoroutine != null)
            {
                StopCoroutine(_stopTrackingCoroutine);
            }

            // If the cat is sleeping, we walk past as normal BUT still track it and kill if it sleeps in the kill zone
            _trackingCat = cat;
        }
    }

    private IEnumerator StopTrackingCat(CatInteractable cat)
    {
        yield return new WaitForSeconds(peripheralTime);
        _trackingCat = null;
        IsChasingCat = false;
        if (cat != null)
        {
            catMarker.position = cat.transform.position;
            _destinationSetter.target = catMarker;
            yield return new WaitUntil(() => _aiPath.reachedEndOfPath);

            var s = (int) _facingController.Direction;
            for (var i = s; i < s + 4; i++)
            {
                var d = (FacingUtils.Direction) (i % 4);
                _facingController.SetFacing(d);
                yield return new WaitForSeconds(lastSeenDelay);
            }
        }
        
        SetPatrolPaused(false);
        _stopTrackingCoroutine = null;
    }
    

    public void OnUnseeCat(CatInteractable cat)
    {
        _stopTrackingCoroutine = StartCoroutine(StopTrackingCat(cat));
        // Use position as to no longer track the cat
        //catMarker.position = cat.transform.position;
        //_destinationSetter.target = catMarker;
        //        SetPatrolPaused(false);

    }

    public void SetPatrolPaused(bool paused)
    {
        _patrolPaused = paused;
        
        if (paused)
        {
            _aiPath.SetPath(null);
            _aiPath.autoRepath.mode = AutoRepathPolicy.Mode.Never;
        }
        else
        {
            _aiPath.autoRepath.mode = AutoRepathPolicy.Mode.Dynamic;
            _aiPath.SearchPath();
        }
    }

    private void Update ()
    {
        if (_trackingCat != null)
        {
            _trackingCat.Interact(gameObject);
        }
        
        if (_patrolPaused || _nodes.Length == 0) return;
        var search = false;

        // Note: using reachedEndOfPath and pathPending instead of reachedDestination here because
        // if the destination cannot be reached by the agent, we don't want it to get stuck, we just want it to get as close as possible and then move on.
        if (_aiPath.reachedEndOfPath && !_aiPath.pathPending && float.IsPositiveInfinity(_switchTime))
        {
            _switchTime = Time.time + patrolDelay;
        }
        
        if (Time.time >= _switchTime)
        {
            _index += 1;
            search = true;
            _switchTime = float.PositiveInfinity;
        }

        _index %= _nodes.Length;
        _destinationSetter.target = _nodes[_index];

        if (search) _aiPath.SearchPath();
    }
}
