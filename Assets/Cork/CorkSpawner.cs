using UnityEngine;

public class CorkSpawner : MonoBehaviour
{
    [SerializeField] private Cork corkPrefab;

    public void Spawn()
    {
        var cork = Instantiate(corkPrefab);
        cork.transform.position = transform.position;
    }
}