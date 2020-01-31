using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] private float[] _floors;
    [SerializeField] private Color[] _debugColors;
    
    public static FloorManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Another singleton instance exists! this should not happen");
            Destroy(Instance);
        }

        Instance = this;
    }

    private void OnDrawGizmos()
    {
        for (var i = 0; i < _floors.Length; i++)
        {
            var color = GetColor(i);
            var floor = _floors[i];
            
            Gizmos.color = color;
            Gizmos.DrawLine(new Vector3(-1000, floor), new Vector3(+1000, floor));
        }
    }

    public IReadOnlyList<IEnumerable<T>> GetObjectsByFloors<T>() where T : Component
    {
        // Create array of (empty) lists
        var result = 
            Enumerable.Range(0, _floors.Length)
            .Select(i => new List<T>()).ToArray();
        
        var objectsInScene = FindObjectsOfType<T>();
        foreach (var obj in objectsInScene)
        {
            var floor = GetFloor(obj);
            if (floor == -1) continue;
            
            result[floor].Add(obj);
        }

        return result;
    }

    private int GetFloor<T>(T component) where T : Component
    {
        var pos = component.transform.position;
        var yPos = pos.y;
        
        // if below ground floor
        if (yPos < _floors[0]) return -1;
        
        for (var i = 0; i < _floors.Length; i++)
        {
            var isTopMost = i == _floors.Length - 1;
            
            var top = isTopMost ? Mathf.Infinity : _floors[i + 1];
            
            if (yPos < top) return i;
        }

        Debug.LogError("How did this happen?");
        return -1;
    }

    public IEnumerable<T> FindObjectsOnFloor<T>(int floor) where T : Component
    {
        var isTopMost = floor == _floors.Length - 1;

        var bottom = _floors[floor];
        var top = isTopMost ? Mathf.Infinity : _floors[floor + 1];

        var objectsInScene = FindObjectsOfType<T>();
        var relevantObjects = objectsInScene.Where(t =>
        {
            var pos = t.transform.position;
            return bottom <= pos.y && pos.y <= top;
        });

        return relevantObjects;
    }

    private Color GetColor(int i)
    {
        if (_debugColors.Length == 0) return Color.white;
        
        i %= _debugColors.Length;
        return _debugColors[i];
    }
}