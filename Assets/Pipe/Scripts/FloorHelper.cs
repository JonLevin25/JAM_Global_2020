using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorHelper : MonoBehaviour
{
    [SerializeField] private float[] _floors;
    [SerializeField] private Color[] _debugColors;
    
    public static FloorHelper Instance;

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
            var floor = Mathf.FloorToInt(GetFloor(obj));
            if (floor == -1) continue;
            
            result[floor].Add(obj);
        }

        return result;
    }

    public float GetFloor<T>(T component) where T : Component
    {
        var pos = component.transform.position;
        var yPos = pos.y;

        return GetFloor(yPos);
    }

    public float GetFloor(float yPos)
    {
        // if below ground floor
        if (yPos < _floors[0]) return -1;
        
        for (var i = 0; i < _floors.Length; i++)
        {
            // For last floor - dont calculate percent, return int
            var isTopMost = i == _floors.Length - 1;
            if (isTopMost) return i;
            
            var top = _floors[i + 1];
            var bottom = _floors[i];

            if (yPos < top)
            {
                var floorPercent = Mathf.InverseLerp(bottom, top, yPos);
                return i + floorPercent;
            }
        }

        Debug.LogError("How did this happen?");
        return -1;
    }

    public IEnumerable<T> FindObjectsOnFloor<T>(int floor) where T : Component
    {
        return GetObjectsByFloors<T>()[floor];
    }

    private Color GetColor(int i)
    {
        if (_debugColors.Length == 0) return Color.white;
        
        i %= _debugColors.Length;
        return _debugColors[i];
    }
}