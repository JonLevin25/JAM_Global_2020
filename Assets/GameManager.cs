using System;
using System.Collections;
using System.Collections.Generic;
using Character.Scripts;
using UnityEngine;

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


    private void Awake()
    {
        gameOverUI.gameObject.SetActive(false);
    }
    
    private IEnumerator Start()
    {
        pipeManager.OnPipeFixed += PipeFixedHandler;
        WaterDrown.Instance.OnDrowned += OnDrowned;
        
        yield return new WaitForSeconds(firstPipeLeakTime);
        LeakPipe(0);
    }

    private void OnDestroy()
    {
        pipeManager.OnPipeFixed -= PipeFixedHandler;
        WaterDrown.Instance.OnDrowned -= OnDrowned;
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
        // TODO: Player death
        GameOver();
    }

    private void GameOver()
    {
        gameOverUI.gameObject.SetActive(true);
    }

    private void PipeFixedHandler(Pipe pipe)
    {
        StartCoroutine(LeakPipeAfter(fixToNextLeakTime));
    }

    private IEnumerator LeakPipeAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        var floodedFloors = waterLevelController.FloodedFloorCount;
        
        LeakPipe(floodedFloors, floodedFloors + 1);
    }

    private void LeakPipe(params int[] fromFloors)
    {
        pipeManager.LeakRandomPipe(fromFloors);
        corkManager.SpawnRandomCork(fromFloors);
    }
}
