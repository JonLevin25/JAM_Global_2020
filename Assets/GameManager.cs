using System;
using System.Collections;
using System.Linq;
using Character.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private PlayerScript player;
    [SerializeField] private WaterLevelController waterLevelController;
    [SerializeField] private PipeManager pipeManager;
    [SerializeField] private CorkManager corkManager;
    [SerializeField] private GameOverController gameOverUI;

    [Header("Level references")]
    [SerializeField] private Pipe _firstPipe;

    [FormerlySerializedAs("firstCorkSpawnTime")]
    [Header("Game Settings")]
    [SerializeField] private float firstPipeBreakTime;
    [SerializeField] private float fixToNextCorkTime;
    [SerializeField] private float floorFloodToNextCorkTime;
    [SerializeField] private float corkToPipeBurstTime;
    [SerializeField] private float gameOverDelay;
    [SerializeField] private float _flashFloodHeightOnLadder = 0.2f;


    private void Awake()
    {
        corkManager = transform.GetComponentInChildren<CorkManager>();
        pipeManager = transform.GetComponentInChildren<PipeManager>();
        waterLevelController = transform.GetComponentInChildren<WaterLevelController>();
        gameOverUI = transform.GetComponentInChildren<GameOverController>();

        gameOverUI.gameObject.SetActive(false);
    }

    private IEnumerator Start()
    {
        pipeManager.OnPipeFixed += OnPipeFixed;
        WaterDrown.OnDrowned += OnDrowned;
        WaterLevelController.Instance.OnFloorFlooded += OnFloorFlooded;
        
        yield return new WaitForSeconds(firstPipeBreakTime);
        pipeManager.LeakPipe(_firstPipe);
        // StartCoroutine(LeakPipe(0));
    }

    private void OnDestroy()
    {
        if(pipeManager)
            pipeManager.OnPipeFixed -= OnPipeFixed;
        WaterDrown.OnDrowned -= OnDrowned;
        WaterLevelController.Instance.OnFloorFlooded += OnFloorFlooded;
    }

    private void OnDrowned(GameObject obj)
    {
        if (obj == player.gameObject)
        {
            OnPlayerDrowned();
        }
    }

    private void OnPlayerDrowned()
    {
        player.Die();
        
        StartCoroutine(GameOver(gameOverDelay));
    }

    private IEnumerator GameOver(float gameOverDelay)
    {
        yield return new WaitForSeconds(gameOverDelay);
        gameOverUI.gameObject.SetActive(true);
    }

    private void OnPipeFixed(Pipe pipe)
    {
        Debug.Log($"OnPipeFixed({pipe.name})");
        StartCoroutine(LeakPipeAfter(fixToNextCorkTime));
    }

    private void OnFloorFlooded(int floor)
    {
        Debug.Log($"OnFloorFlooded({floor}) [TopFloor: {FloorHelper.Instance.TopFloor}]");
        if (floor == FloorHelper.Instance.TopFloor) return; // Player's gonna die soon anyway
        
        pipeManager.ClosePipesOnFloor(floor);
        StartCoroutine(FlashFloodFloorWhenReady(floor));
        StartCoroutine(LeakPipeAfter(floorFloodToNextCorkTime));
    }

    public IEnumerator FlashFloodFloorWhenReady(int floor)
    {
        bool Condition()
        {
            var playerOnLadder = player.currentLadder != null;
            var playerFloor = player.GetFloorWithPercent();

            var minLadderHeight = floor + _flashFloodHeightOnLadder;
            var playerHighEnoughOnLadder = playerOnLadder &&  playerFloor > minLadderHeight;
            var playerAboveFloor = player.GetFloor() > floor + 1;
            
            return playerHighEnoughOnLadder || playerAboveFloor;
        }

        // If top floor - flood immediately
        if (floor == FloorHelper.Instance.TopFloor)
        {
            WaterLevelController.Instance.FlashFloodLevel(floor);
        }
        
        while (true)
        {
            if (Condition())
            {
                WaterLevelController.Instance.FlashFloodLevel(floor);
                break;
            }
            yield return null;
        }
    }

    private IEnumerator LeakPipeAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        var highestFlooded = waterLevelController.HighestFloodedFloor;
        

        var topFloor = FloorHelper.Instance.TopFloor;
        if (highestFlooded == topFloor - 1)
        {
            StartCoroutine(LeakPipe(topFloor));
        }
        else
        {
            StartCoroutine(LeakPipe(highestFlooded + 1, highestFlooded + 2));
        }
    }

    private IEnumerator LeakPipe(params int[] fromFloors)
    {
        Debug.Log($"Choosing Random Leak + Cork from floors: {string.Join(", ", fromFloors)}");

        var playerHoldingCork = player.currentCork; 
        if (!playerHoldingCork)
        {
            var corkSpawner = corkManager.GetRandomSpawner(fromFloors);
            corkSpawner.Spawn();
            
            // If cork spawned - make sure pipe is on same floor and wait
            fromFloors = new[] { corkSpawner.GetFloor()};
            yield return new WaitForSeconds(corkToPipeBurstTime);
        }

        // Filter any floors that are already flooded
        fromFloors = fromFloors
            .Where(floor => !waterLevelController.IsFloorFlooded(floor))
            .ToArray();

        // If all floors we wanted were flooded, spawn in first non-flooded
        if (fromFloors.Length == 0)
        {
            // If all floors flooded, fuck it
            if (waterLevelController.AllFloorsFlooded) yield break;
            fromFloors = new[] {waterLevelController.HighestFloodedFloor + 1};
        }
        
        pipeManager.LeakRandomPipe(fromFloors);
    }
}
