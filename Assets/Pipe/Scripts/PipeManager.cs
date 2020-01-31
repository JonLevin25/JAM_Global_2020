using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PipeManager : MonoBehaviour
{
    [SerializeField] private bool _debug;
    
    private IReadOnlyList<List<Pipe>> _pipesByFloor;
    private readonly HashSet<Pipe> _fixedPipe = new HashSet<Pipe>();
    private HashSet<Pipe> _currLeakingPipes = new HashSet<Pipe>();

    public static PipeManager Instance;
    
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
        foreach (var pipe in _fixedPipe)
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

    public void LeakRandomPipe(params int[] fromFloors)
    {
        var allPipesOnFloors = fromFloors.SelectMany(i => _pipesByFloor[i]);
        var relevantPipes = allPipesOnFloors.Where(NotFixedOrLeaking).ToArray(); // disregard fixed or currently leaking pipes

        var pipeIdx = Random.Range(0, relevantPipes.Length - 1);
        var selectedPipe = relevantPipes[pipeIdx];
        
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
        _fixedPipe.Add(pipe);
        _currLeakingPipes.Remove(pipe);
    }

    private bool NotFixedOrLeaking(Pipe pipe) => !IsFixed(pipe) && !IsLeaking(pipe);

    private bool IsFixed(Pipe pipe) => _fixedPipe.Contains(pipe);
    private bool IsLeaking(Pipe pipe) => _currLeakingPipes.Contains(pipe);
}
