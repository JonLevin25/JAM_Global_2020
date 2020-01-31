using System;
using UnityEngine;

public class WaterLevelController : MonoBehaviour
{
    [SerializeField] private float _startWaterLevel;
    [SerializeField] private float _smoothTime;
    [SerializeField] private WaterLevelView _water;

    [SerializeField] private bool _TEST_setWater;
    [SerializeField] private float _TEST_targetWaterLevel;

    public static WaterLevelController Instance;

    private float _currWaterLevel;
    private float _targetWaterLevel;
    private float _velocity;
    private int _prevHighestFloodedFloor = -1;

    public int HighestFloodedFloor
    {
        get
        {
            var waterLevel = FloorHelper.Instance.GetFloor(_currWaterLevel);
            return Mathf.FloorToInt(waterLevel - 0.5f); // Return the last floor who'se more than half flooded
        }
    }

    public float CurrWaterLevel => _currWaterLevel;

    public event Action<int> OnFloorFlooded;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Another singleton instance exists! this should not happen");
            Destroy(Instance);
        }

        Instance = this;
    }

    private void Start()
    {
        _targetWaterLevel = _startWaterLevel;
    }
    
    private void Update()
    {
        // Debugging/Testing
        if (_TEST_setWater)
        {
            _targetWaterLevel = _TEST_targetWaterLevel;
        }
        
        // Smooth water to level
        _currWaterLevel = Mathf.SmoothDamp(_currWaterLevel, _targetWaterLevel, ref _velocity, _smoothTime);
        _water.SetWaterLevel(_currWaterLevel);

        if (HighestFloodedFloor > _prevHighestFloodedFloor)
        {
            var newFloorFlooded = _prevHighestFloodedFloor + 1;
            OnFloorFlooded?.Invoke(newFloorFlooded);
            
            Debug.Log($"OnFloorFlooded({newFloorFlooded}) [WaterLevel: {_currWaterLevel}]");
            _prevHighestFloodedFloor = newFloorFlooded;
        }
    }

    public void IncreaseWaterLevel(float delta)
    {
        _targetWaterLevel += delta;
    }
}
