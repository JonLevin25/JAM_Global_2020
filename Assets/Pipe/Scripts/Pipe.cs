using System;
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
}

public class Pipe : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _rend;
    [SerializeField] private FlowPhase[] phases;
    private Coroutine _flowRoutine;
    private float _flowRate;
    private bool _isLeaking;

    public event Action<Pipe> OnPipeFixed;
    public bool IsLeaking => _isLeaking;

    private void Update()
    {
        if (_flowRate > 0)
        {
            WaterLevelController.Instance.IncreaseWaterLevel(_flowRate * Time.deltaTime);
        }
        
    }

    public void FixPipe()
    {
        OnPipeFixed?.Invoke(this);
    }

    public void StartFlow()
    {
        _flowRoutine = StartCoroutine(FlowRoutine());
        _isLeaking = true;
    }

    public void StopFlow()
    {
        if (_flowRoutine != null) StopCoroutine(_flowRoutine);
        _flowRate = 0;
        _rend.color = Color.white;
        _isLeaking = false;
    }

    private IEnumerator FlowRoutine()
    {
        var startTime = Time.time;
        foreach (var phase in phases)
        {
            var timeFromFromStart = Time.time - startTime;
            while (timeFromFromStart < phase.startTime)
            {
                timeFromFromStart = Time.time - startTime;
                yield return null;
            }

            SetPhase(phase);
        }
    }

    private void SetPhase(FlowPhase phase)
    {
        _flowRate = phase.rate;
        _rend.color = phase.testColor;
    }
}
