using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class WaterLevelView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _rend;
    [SerializeField] private Transform _colliderTrans;
    
    private const string ShaderIntensity = "_Intensity";
    private const string ShaderSpeed = "_Speed";
    
    public void SetWaterLevel(float level)
    {
        // Set scale by level
        UpdateSpriteSize(level);
        UpdateColliderSize(level);
    }

    public void SetShaderIntensity(float intensity)
    {
        _rend.material.SetFloat(ShaderIntensity, intensity);
    }

    public void SetShaderSpeed(float speed)
    {
        _rend.material.SetFloat(ShaderSpeed, speed);
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
