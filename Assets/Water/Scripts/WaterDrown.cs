using System;
using UnityEngine;

public class WaterDrown : MonoBehaviour
{
    [SerializeField] private bool _debug;
    [SerializeField] private float _timeToDrown;
    [SerializeField] private Transform _drownMarker;

    public static event Action<GameObject> OnDrowned;
    private float _underWaterCounter;
    private bool _hasDrowned;

    public static WaterDrown Instance;

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
            fontSize = 30
        };
        GUILayout.Label("Drowned Time", style);
    }

    private void Update()
    {
        if (_hasDrowned) return;
        
        var height = _drownMarker.position.y;
        var waterHeight = WaterLevelController.Instance.WorldWaterLevel;
        
        if (height >= waterHeight) return; // We're not under water
        
        
        _underWaterCounter += Time.deltaTime;
        if (_underWaterCounter >= _timeToDrown)
        {
            OnDrownedInternal();
        }
    }

    private void OnDrownedInternal()
    {
        OnDrowned?.Invoke(gameObject);
        _hasDrowned = true;
        // _underWaterCounter = 0f;
    }
    

    private static bool ContainsLayer(LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }
}
