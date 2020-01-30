using UnityEngine;

public class WaterLevelView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _rend;
    [SerializeField] private Transform _colliderTrans;
    
    public void SetWaterLevel(float level)
    {
        // Set scale by level
        UpdateSpriteSize(level);
        UpdateColliderSize(level);
    }

    private void UpdateSpriteSize(float level)
    {
        var scale = _rend.size;
        scale.y = level;
        _rend.size = scale;
    }

    private void UpdateColliderSize(float level)
    {
        var scale = _colliderTrans.localScale;
        scale.y = level;
        _colliderTrans.localScale = scale;
    }
}
