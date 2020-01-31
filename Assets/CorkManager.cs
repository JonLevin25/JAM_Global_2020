using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorkManager : MonoBehaviour
{
    private IReadOnlyList<IReadOnlyList<CorkSpawner>> _corksByFloor;

    private void Start()
    {
        _corksByFloor = FloorHelper.Instance.GetObjectsByFloors<CorkSpawner>()
            .Select(corks => corks.ToArray())
            .ToArray();
    }

    public void SpawnRandomCork(params int[] fromFloors)
    {
        if (fromFloors.Length == 0)
        {
            Debug.LogError($"{GetType()}.{nameof(SpawnRandomCork)} called with no floors!");
            return;
        }
        
        var allPipesOnFloors = fromFloors.SelectMany(i => _corksByFloor[i]);
        var relevantPipes = allPipesOnFloors.Where(CorkFilterCondition).ToArray(); // disregard fixed or currently leaking pipes

        if (relevantPipes.Length == 0)
        {
            Debug.Log($"{GetType()}.{nameof(SpawnRandomCork)}: no relevant found!");
            return;
        }
        
        var idx = Random.Range(0, relevantPipes.Length - 1);
        var selected = relevantPipes[idx];
        
        Debug.Log($"{GetType()}.{nameof(SpawnRandomCork)}: selected ({selected.name})");
        selected.Spawn();
    }
    private bool CorkFilterCondition(CorkSpawner arg)
    {
        return true; // Don't filter corks right now
    }
}
