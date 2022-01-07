using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Serialization;

public class GuardAI : MonoBehaviour
{
    // Comp
    private AIDestinationSetter _destinationSetter;
    private AIPath _aiPath;
    [HideInInspector]
    public SpeechController speechController;
    private AudioSource _audio;
    private FacingController _facingController;
    private ScoreManager _scoreManager;
    private CatInteractable _cat;

    private ViewConeController _viewCone;
    private ViewConeController _killCone;

    // Params
    public Transform catMarker;
    public GameObject path;
    public float patrolDelay = 0f;
    public float peripheralTime = 0.5f;
    public float lastSeenDelay = 0.5f;
    public float scoreDifficultyScaling = 0.01f;

    public AudioClip spottedSound;

    private bool IsChasingCat { get; set; } = false;

    // Backing Props
    private bool _patrolPaused = false;
    private Transform[] _nodes;
    private float _switchTime = float.PositiveInfinity;
    private int _index = 0;
    public Coroutine StopTrackingCoroutine;

    private void Awake()
    {
        speechController = GetComponentInChildren<SpeechController>();
        _aiPath = GetComponent<AIPath>();
        _audio = GetComponent<AudioSource>();
        _facingController = GetComponent<FacingController>();
        _destinationSetter = GetComponent<AIDestinationSetter>();
        _viewCone = transform.Find("ViewCone").GetComponent<ViewConeController>();
        _killCone = transform.Find("KillerViewCone").GetComponent<ViewConeController>();
        _scoreManager = ScoreManager.Get();
        _scoreManager.scoreChanged.AddListener(OnScoreChanged);

        if (path != null)
        {
            _nodes = new Transform[path.transform.childCount];

            for (var i = 0; i < path.transform.childCount; i++)
            {
                _nodes[i] = path.transform.GetChild(i).transform;
            }

            if (_nodes.Length > 0)
            {
                // Teleport to start of path on awake
                transform.position = _nodes[0].position;
            }
        }
    }

    private void OnScoreChanged(int score, int change)
    {
        // Using this graph I designed: https://www.desmos.com/calculator/tlmfgxx9pl
        var old = _viewCone.Radius;
        _viewCone.Radius = Mathf.Log(scoreDifficultyScaling * score + 1f) + _viewCone.initialRadius;
        Debug.Log("New guard ViewCone r:" + _viewCone.Radius + ", prev: " + old);
    }

    public bool StartChasingCat(CatInteractable cat)
    {
        var d = (cat.transform.position - transform.position).magnitude;
        // I.e. don't start a new chase if it's about to be killed via the kill cone or if already chasing or if sleeping
        if (IsChasingCat || cat.controller.IsSleeping || d <= _killCone.Radius)
        {
            return false;
        }
        IsChasingCat = true;
        SetPatrolPaused(true);
        _destinationSetter.target = cat.transform;
        _aiPath.autoRepath.mode = AutoRepathPolicy.Mode.Dynamic;
        _aiPath.SearchPath();
        speechController.Say("!", Color.red, size: 12, anim: SpeechController.AnimDoGrow);
        _audio.PlayOneShot(spottedSound);
        return true;
    }

    public void OnSeeCat(CatInteractable cat)
    {
        _cat = cat;
    }

    private IEnumerator StopTrackingCat(CatInteractable cat)
    {
        yield return new WaitForSeconds(peripheralTime);
        IsChasingCat = false;

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
        
        
        SetPatrolPaused(false);
        StopTrackingCoroutine = null;
    }
    

    public void OnUnseeCat(CatInteractable cat)
    {
        _cat = null;
        StopTrackingCoroutine = StartCoroutine(StopTrackingCat(cat));
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

    private void Chase(CatInteractable cat)
    {
        if (StartChasingCat(cat) && StopTrackingCoroutine != null)
        {
            StopCoroutine(StopTrackingCoroutine);
        }
    }

    private void Update ()
    {
        if (_cat != null)
        {
            Chase(_cat);
        }
        
        if (_patrolPaused || path == null || _nodes.Length == 0) return;
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
