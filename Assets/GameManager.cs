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
    [SerializeField] private float firstPipeLeakTime;
    [SerializeField] private float fixToNextLeakTime;
    [SerializeField] private float floorFloodToNextLeakTime;
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
        pipeManager.OnPipeFixed += PipeFixedHandler;
        WaterDrown.OnDrowned += OnDrowned;
        WaterLevelController.Instance.OnFloorFlooded += OnFloorFlooded;
        
        yield return new WaitForSeconds(firstPipeLeakTime);
        LeakPipe(0);
    }

    private void OnDestroy()
    {
        pipeManager.OnPipeFixed -= PipeFixedHandler;
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

    private void PipeFixedHandler(Pipe pipe)
    {
        StartCoroutine(LeakPipeAfter(fixToNextLeakTime));
    }

    private void OnFloorFlooded(int floor)
    {
        if (floor == FloorHelper.Instance.TopFloor) return; // Player's gonna die soon anyway
        
        pipeManager.ClosePipesOnFloor(floor);
        StartCoroutine(LeakPipeAfter(floorFloodToNextLeakTime));
    }

    private IEnumerator LeakPipeAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        var highestFlooded = waterLevelController.HighestFloodedFloor;

        LeakPipe(highestFlooded + 1, highestFlooded + 2);
    }

    private void LeakPipe(params int[] fromFloors)
    {
        pipeManager.LeakRandomPipe(fromFloors);
        corkManager.SpawnRandomCork(fromFloors);
    }
}
