using System;
using System.Collections.Generic;
using UnityEngine;

public class WaterDrown : MonoBehaviour
{
    [SerializeField] private bool _debug;
    [SerializeField] private float _timeToDrown;
    
    public static event Action<GameObject> OnDrowned;
    
    private Dictionary<GameObject, float> _drownTime = new Dictionary<GameObject, float>();
    private HashSet<GameObject> _drownedObjects = new HashSet<GameObject>();

    private void OnGUI()
    {
        if (!_debug) return;
        
        var style = new GUIStyle
        {
            fontSize = 30
        };
        GUILayout.Label("Drowned Time", style);
        foreach (var kvp in _drownTime)
        {
            GUILayout.Label($"{kvp.Key.name} [{kvp.Value}]", style);
        }
        
        GUILayout.Space(50);
        
        GUILayout.Label("Already Drowned", style);
        foreach (var obj in _drownedObjects)
        {
            GUILayout.Label($"{obj.name}", style);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        var otherGO = other.gameObject;
        
        // Already drowned
        if (_drownedObjects.Contains(otherGO)) return;
            
        if (_drownTime.ContainsKey(otherGO))
        {
            _drownTime[otherGO] += Time.deltaTime;
            if (_drownTime[otherGO] >= _timeToDrown)
            {
                OnObjectDrowned(otherGO);
            }
        }
        else
        {
            _drownTime.Add(otherGO, 0f);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var otherGO = other.gameObject;
        if (_drownTime.ContainsKey(otherGO))
        {
            _drownTime.Remove(other.gameObject);
        }
    }

    private void OnObjectDrowned(GameObject otherGo)
    {
        OnDrowned?.Invoke(otherGo);
        _drownTime.Remove(otherGo);
        _drownedObjects.Add(otherGo);
    }
}
