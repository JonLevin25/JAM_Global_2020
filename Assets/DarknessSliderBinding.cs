using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DarknessSliderBinding : MonoBehaviour
{
    [SerializeField] private Light2D _light;
    
    [Header("Intensity values")]
    [SerializeField] private float _min;
    [SerializeField] private float _max;
    [SerializeField] private float _default;
    
    public static float Brightness
    {
        get { return PlayerPrefs.GetFloat("brightness", -1); }
        set
        {
            PlayerPrefs.SetFloat("brightness", value);
            OnSet?.Invoke(value);
        }
    }

    public static Action<float> OnSet;
    
    private void Awake()
    {
        OnSet += SetBrightnessInstance;
        var brightness = Brightness;
        if (brightness < 0)
        {
            brightness = _default;
        }
        
        SetBrightnessInstance(brightness);
    }

    private void OnDestroy()
    {
        OnSet -= SetBrightnessInstance;
    }

    private void SetBrightnessInstance(float level)
    {
        var intensity = Mathf.Lerp(_min, _max, level);
        _light.intensity = intensity;
    }
}
