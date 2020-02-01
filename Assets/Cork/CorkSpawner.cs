using UnityEngine;

public class CorkSpawner : MonoBehaviour
{
    [SerializeField] private Cork corkPrefab;
    [SerializeField] private float randomForcePower;
    [SerializeField] private Vector2 randomAngleRange;

    public void Spawn()
    {
        var cork = Instantiate(corkPrefab);
        cork.transform.Rotate(new Vector3(0f, 0f, Random.Range(randomAngleRange[0], randomAngleRange[1])));
        cork.transform.position = transform.position;
        
        var randomForce = randomForcePower * Random.insideUnitCircle;
        cork.rigidbody.AddForce(randomForce, ForceMode2D.Impulse);
    }
}