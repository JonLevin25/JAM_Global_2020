using System;
using System.Collections;
using System.Collections.Generic;
using Character.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerScript player;
    [SerializeField] private WaterLevelController waterLevelController;
    [SerializeField] private PipeManager pipeManager;
    [SerializeField] private CorkManager corkManager;
    [SerializeField] private GameOverController gameOverUI;

    [Header("Game Settings")]
    [FormerlySerializedAs("firstPipeLeakTime")]
    [SerializeField] private float firstCorkSpawnTime;
    [FormerlySerializedAs("fixToNextLeakTime"), SerializeField] private float fixToNextCorkTime;
    [FormerlySerializedAs("floorFloodToNextLeakTime")] [SerializeField] private float floorFloodToNextCorkTime;
    [SerializeField] private float corkToPipeBurstTime;
    [FormerlySerializedAs("_gameOverDelay")] [SerializeField] private float gameOverDelay;
    


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
        
        yield return new WaitForSeconds(firstCorkSpawnTime);
        StartCoroutine(LeakPipe(0));
    }

    private void OnDestroy()
    {
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
        StartCoroutine(LeakPipeAfter(floorFloodToNextCorkTime));
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
        var corkSpawner = corkManager.GetRandomSpawner(fromFloors);
        corkSpawner.Spawn();
        
        var floor = corkSpawner.GetFloor();
        yield return new WaitForSeconds(corkToPipeBurstTime);
        pipeManager.LeakRandomPipe(floor);
    }
}
