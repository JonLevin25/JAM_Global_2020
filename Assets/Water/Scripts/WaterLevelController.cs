using System;
using UnityEngine;

public class WaterLevelController : MonoBehaviour
{
    [SerializeField] private float _startWaterLevel;
    [SerializeField] private float _smoothTime;
    [SerializeField] private WaterLevelView _water;

    [SerializeField] private bool _TEST_setWater;
    [SerializeField] private float _TEST_targetWaterLevel;
    
    
    [SerializeField] private int _TEST_flashFloodLevel;

    public static WaterLevelController Instance;

    private float _currWaterLevel;
    private float _targetWaterLevel;
    private float _velocity;
    private int _prevHighestFloodedFloor = -1;

    public float WorldWaterLevel => _water.transform.position.y + _currWaterLevel;

    public int HighestFloodedFloor
    {
        get
        {
            var waterFloor = FloorHelper.Instance.GetFloorByHeight(WorldWaterLevel);
            if (waterFloor < 0) return -1;
            
            return Mathf.FloorToInt(waterFloor - 0.5f); // Return the last floor whose more than half flooded
        }
    }

    public bool AllFloorsFlooded => HighestFloodedFloor >= FloorHelper.Instance.TopFloor;
    
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
            _prevHighestFloodedFloor++;
            var newFloorFlooded = _prevHighestFloodedFloor;
            OnFloorFlooded?.Invoke(newFloorFlooded);
        }
    }

    public void IncreaseWaterLevel(float delta)
    {
        _targetWaterLevel += delta;
    }

    [ContextMenu("TEST Flash Flood ")]
    public void TEST_FlashFlood() => FlashFloodLevel(_TEST_flashFloodLevel);

    public void FlashFloodLevel(int level)
    {
        Debug.Log($"FlashFlood! floor: ({level})");
        var topFloorNum = FloorHelper.Instance.TopFloor;
        if (level >= topFloorNum)
        {
            FlashFloodFinalFloor(topFloorNum);
        }
        else
        {
            var nextLevel = level + 1;
            var worldTargetLevel = FloorHelper.Instance.GetFloorByIndex(nextLevel);
            _targetWaterLevel = WorldToLocalWaterLevel(worldTargetLevel);
        }
    }

    private void FlashFloodFinalFloor(int topFloorNum)
    {
        Debug.Log($"FlashFlood final floor! ({topFloorNum})");
        var topFloor = FloorHelper.Instance.GetFloorByIndex(topFloorNum);
        var beforeTopFloor = FloorHelper.Instance.GetFloorByIndex(topFloorNum - 1);

        var floorDelta = topFloor - beforeTopFloor;
        if (_currWaterLevel > FloorHelper.Instance.TopFloor)
        {
            var worldTargetLevel = topFloorNum + 2 * floorDelta;
            _targetWaterLevel = WorldToLocalWaterLevel(worldTargetLevel);
        }
    }

    private float WorldToLocalWaterLevel(float worldHeight)
    {
        return worldHeight - _water.transform.position.y;
    }

    public bool IsFloorFlooded(int floor)
    {
        return HighestFloodedFloor >= floor;
    }
}
