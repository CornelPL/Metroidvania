using UnityEngine;
using MyBox;

public class AutoLight : MonoBehaviour
{
    [SerializeField, MustBeAssigned] private UnityEngine.Experimental.Rendering.Universal.Light2D _light = null;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float maxIntensity = 1f;
    [SerializeField] private float minIntensity = 1f;


    private bool isIncreasing = true;


    private void Update()
    {
        if ( isIncreasing )
        {
            _light.intensity += speed * Time.deltaTime;
            if ( _light.intensity > maxIntensity )
            {
                isIncreasing = false;
            }
        }
        else
        {
            _light.intensity -= speed * Time.deltaTime;
            if ( _light.intensity < minIntensity )
            {
                isIncreasing = true;
            }
        }
    }
}
