using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PipeManager : MonoBehaviour
{
    [Tooltip("The number of \"last pipes\" to ignore when choosing a new one")]
    [SerializeField] private int _disallowRepeatLastPipes = 1;
    [SerializeField] private bool _debug;
    [SerializeField] private int[] _debugFloors;
    
    // public static PipeManager Instance;
    
    private IReadOnlyList<List<Pipe>> _pipesByFloor;
    private readonly Stack<Pipe> _fixedPipes = new Stack<Pipe>();
    private readonly HashSet<Pipe> _currLeakingPipes = new HashSet<Pipe>();

    public event Action<Pipe> OnPipeFixed;

    // private void Awake()
    // {
    //     if (Instance != null)
    //     {
    //         Debug.LogError("Another singleton instance exists! this should not happen");
    //         Destroy(Instance);
    //     }
    //
    //     Instance = this;
    // }
    
    private void OnGUI()
    {
        if (!_debug) return;

        if (GUILayout.Button("LeakRandom"))
        {
            LeakRandomPipe(_debugFloors);
        }

        var style = new GUIStyle
        {
            fontSize = 20
        };
        
        GUILayout.Label("Leaking Pipes:", style);
        foreach (var pipe in _currLeakingPipes)
        {
            GUILayout.Label(pipe.name, style);
        }
        
        GUILayout.Space(25f);
        
        GUILayout.Label("Last Pipes fixed:", style);
        foreach (var pipe in _fixedPipes)
        {
            GUILayout.Label(pipe.name, style);
        }
        
    }

    private void Start()
    {
        _pipesByFloor = FloorHelper.Instance.GetObjectsByFloors<Pipe>()
            .Select(pipes => pipes.ToList()).ToList();
        
        foreach (var pipe in _pipesByFloor.SelectMany(pipes => pipes))
        {
            pipe.OnPipeFixed += PipeFixedHandler;
        }
    }

    public void LeakRandomPipe(params int[] fromFloors)
    {
        if (fromFloors.Length == 0)
        {
            Debug.LogError("LeakRandomPipe called with no floors!");
            return;
        }
        
        var allPipesOnFloors = fromFloors.SelectMany(i => _pipesByFloor[i]);
        var relevantPipes = allPipesOnFloors.Where(PipeFilterCondition).ToArray(); // disregard fixed or currently leaking pipes

        if (relevantPipes.Length == 0)
        {
            Debug.Log($"{GetType()}.{nameof(LeakRandomPipe)}: no relevant pipes found!");
            return;
        }
        
        var pipeIdx = Random.Range(0, relevantPipes.Length - 1);
        var selectedPipe = relevantPipes[pipeIdx];
        
        Debug.Log($"{GetType()}.{nameof(LeakRandomPipe)}: selected pipe ({selectedPipe.name})");
        LeakPipe(selectedPipe);
    }

    private void LeakPipe(Pipe pipe)
    {
        pipe.StartFlow();
        _currLeakingPipes.Add(pipe);
        SetPipeState(pipe, LeakState.Leaking);
    }

    private void PipeFixedHandler(Pipe pipe)
    {
        pipe.StopFlow();
        _fixedPipes.Push(pipe);
        _currLeakingPipes.Remove(pipe);
        OnPipeFixed?.Invoke(pipe);
        SetPipeState(pipe, LeakState.Sealed);
    }

    private bool PipeFilterCondition(Pipe pipe)
    {
        if (IsLeaking(pipe)) return false;
        foreach (var fixedPipe in _fixedPipes.Take(_disallowRepeatLastPipes))
        {
            if (fixedPipe == pipe) return false;
        }

        return true;
    }

    private void SetPipeState(Pipe p, LeakState l)
    {
        p.SetPipeState(l);
    }

    private bool IsFixed(Pipe pipe) => _fixedPipes.Contains(pipe);
    private bool IsLeaking(Pipe pipe) => _currLeakingPipes.Contains(pipe);

    public void ClosePipesOnFloor(int floor)
    {
        foreach (var pipe in _pipesByFloor[floor])
        {
            pipe.StopFlow();
            pipe.SetPipeState(LeakState.NoLeak);
        }
    }
}
