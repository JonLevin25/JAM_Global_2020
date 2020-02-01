﻿using System;
using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Pipe))]
public class PipeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = target as Pipe;

        if (GUILayout.Button("StartFlow"))
        {
            script.StartFlow();
        }

        if (GUILayout.Button("StopFlow"))
        {
            script.StopFlow();
        }
    }
}
#endif

[Serializable]
public struct FlowPhase
{
    public float startTime;
    public Color testColor;
    public float rate;
    public AudioClip FlowSound;
    
}
public enum LeakState
{
    NoLeak = 0,
    Leaking = 1,
    Sealed = 2
}

public class Pipe : MonoBehaviour
{
    [SerializeField] LeakState LeakingStatus;
    [SerializeField] private FlowPhase[] phases;

    [SerializeField] private bool _debug;
    [SerializeField] private Color _debugSealedColor;

    [SerializeField] private ParticleSystem lightFlowParticles;
    [SerializeField] private ParticleSystem heavyFlowParticles;
    
    private Animator _myAnim;
    private SpriteRenderer _rend;
    private AudioSource _Audio;
    private Coroutine _flowRoutine;
    private float _flowRate;
    private bool _isLeaking;
    int _CurrentLeakingStatus = 0;
    int _LastLeakingStatus = 0;

    public event Action<Pipe> OnPipeFixed;
    public bool IsLeaking => _isLeaking;

    private void Awake()
    {
        _rend = GetComponent<SpriteRenderer>();
        _myAnim = GetComponent<Animator>();
        _Audio = GetComponent<AudioSource>();

    }

    private void Start()
    {
        SetPipeState(LeakState.NoLeak);
    }
    private void Update()
    {
        if (_flowRate > 0)
        {
            Debug.Log($"Pipe {name} Increasing water!");
            WaterLevelController.Instance.IncreaseWaterLevel(_flowRate * Time.deltaTime);
        }
        if(_LastLeakingStatus!=_CurrentLeakingStatus)
        {
            _LastLeakingStatus = _CurrentLeakingStatus;
            _myAnim.SetInteger("state", _LastLeakingStatus);
        }
    }

    public void FixPipe()
    {
        OnPipeFixed?.Invoke(this);
        if (_debug)
        {
            _rend.color = _debugSealedColor;
        }
    }

    public void StartFlow()
    {
        _flowRoutine = StartCoroutine(FlowRoutine());
        _isLeaking = true;
        _Audio.Play();
        lightFlowParticles?.Play();
    }

    public void StopFlow()
    {
        if (_flowRoutine != null) StopCoroutine(_flowRoutine);
        _flowRate = 0;
        _rend.color = Color.white;
        _isLeaking = false;
        _Audio.Stop();
        lightFlowParticles?.Stop();
        heavyFlowParticles?.Stop();
    }

    private IEnumerator FlowRoutine()
    {
        var startTime = Time.time;
        for (var i = 0; i < phases.Length; i++)
        {
            var phase = phases[i];
            var timeFromFromStart = Time.time - startTime;
            while (timeFromFromStart < phase.startTime)
            {
                timeFromFromStart = Time.time - startTime;
                yield return null;
            }

            SetPhase(phase, i);
        }
    }

    private void SetPhase(FlowPhase phase, int i)
    {
        _flowRate = phase.rate;
        _Audio.clip = phase.FlowSound;
        _Audio.loop = true;
        _Audio.Play();

        if (i > 0)
        {
            heavyFlowParticles?.Play();
        }
        
        if (_debug)
        {
            _rend.color = phase.testColor;
        }
    }

    public void SetPipeState(LeakState state)
    {
        _CurrentLeakingStatus = (int)state;

    }
}
