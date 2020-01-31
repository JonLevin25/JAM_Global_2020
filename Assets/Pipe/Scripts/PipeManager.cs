using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class PipeManager : MonoBehaviour
{
    [SerializeField] private bool _debug;
    [SerializeField] private int[] _debugFloors;
    
    public static PipeManager Instance;
    
    private IReadOnlyList<List<Pipe>> _pipesByFloor;
    private readonly HashSet<Pipe> _fixedPipes = new HashSet<Pipe>();
    private readonly HashSet<Pipe> _currLeakingPipes = new HashSet<Pipe>();

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Another singleton instance exists! this should not happen");
            Destroy(Instance);
        }

        Instance = this;
    }
    
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
        
        GUILayout.Label("Fixed Pipes:", style);
        foreach (var pipe in _fixedPipes)
        {
            GUILayout.Label(pipe.name, style);
        }
        
    }

    private void Start()
    {
        _pipesByFloor = FloorManager.Instance.GetObjectsByFloors<Pipe>()
            .Select(pipes => pipes.ToList()).ToList();

        foreach (var pipe in _pipesByFloor.SelectMany(pipes => pipes))
        {
            pipe.OnPipeFixed += OnPipeFixed;
        }
    }

    public void LeakRandomPipe(IEnumerable<int> fromFloors)
    {
        var allPipesOnFloors = fromFloors.SelectMany(i => _pipesByFloor[i]);
        var relevantPipes = allPipesOnFloors.Where(NotFixedOrLeaking).ToArray(); // disregard fixed or currently leaking pipes

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
    }

    private void OnPipeFixed(Pipe pipe)
    {
        pipe.StopFlow();
        _fixedPipes.Add(pipe);
        _currLeakingPipes.Remove(pipe);
    }

    private bool NotFixedOrLeaking(Pipe pipe) => !IsFixed(pipe) && !IsLeaking(pipe);

    private bool IsFixed(Pipe pipe) => _fixedPipes.Contains(pipe);
    private bool IsLeaking(Pipe pipe) => _currLeakingPipes.Contains(pipe);
}
