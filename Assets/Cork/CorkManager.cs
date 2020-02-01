using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorkManager : MonoBehaviour
{
    private IReadOnlyList<IReadOnlyList<CorkSpawner>> _corksByFloor;
    private int _lastCorkFloor;
    public CorkSpawner LastCorkSpawnerReturned { get; private set; }

    private void Start()
    {
        _corksByFloor = FloorHelper.Instance.GetObjectsByFloors<CorkSpawner>()
            .Select(corks => corks.ToArray())
            .ToArray();
    }

    public void SpawnRandomCork(params int[] fromFloors)
    {
        var cork = GetRandomSpawner(fromFloors);
        cork.Spawn();
    }

    public CorkSpawner GetRandomSpawner(params int[] fromFloors)
    {
        if (fromFloors.Length == 0)
        {
            Debug.LogError($"{GetType()}.{nameof(GetRandomSpawner)} called with no floors!");
            return null;
        }
        
        var allPipesOnFloors = fromFloors.SelectMany(i => _corksByFloor[i]);
        var relevantPipes = allPipesOnFloors.Where(CorkFilterCondition).ToArray(); // disregard fixed or currently leaking pipes

        if (relevantPipes.Length == 0)
        {
            Debug.Log($"{GetType()}.{nameof(GetRandomSpawner)}: no relevant found!");
            return null;
        }
        
        var idx = Random.Range(0, relevantPipes.Length - 1);
        var selectedSpawner = relevantPipes[idx];
        
        Debug.Log($"{GetType()}.{nameof(GetRandomSpawner)}: selected ({selectedSpawner.name})");

        LastCorkSpawnerReturned = selectedSpawner;
        return selectedSpawner;
    }


    private bool CorkFilterCondition(CorkSpawner arg)
    {
        return true; // Don't filter corks right now
    }
}
