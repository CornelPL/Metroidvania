using UnityEngine;

public class ExplosionLight : MonoBehaviour
{
    [SerializeField] private UnityEngine.Experimental.Rendering.LWRP.Light2D _light = null;

    [HideInInspector] public bool fadeIn = false;
    [HideInInspector] public float fadeInTime = 1f;

    [HideInInspector] public bool fadeInOuterRadius = false;
    [HideInInspector] public float minInOuterRadius = 0f;
    [HideInInspector] public float maxInOuterRadius = 10f;

    [HideInInspector] public bool fadeInIntensity = false;
    [HideInInspector] public float minInIntensity = 0f;
    [HideInInspector] public float maxInIntensity = 10f;


    [HideInInspector] public bool fadeOut = false;
    [HideInInspector] public float fadeOutTime = 1f;

    [HideInInspector] public bool fadeOutOuterRadius = false;
    [HideInInspector] public float minOutOuterRadius = 0f;
    [HideInInspector] public float maxOutOuterRadius = 10f;

    [HideInInspector] public bool fadeOutIntensity = false;
    [HideInInspector] public float minOutIntensity = 0f;
    [HideInInspector] public float maxOutIntensity = 10f;

    [HideInInspector] public bool destroyOnEnd = false;


    public void Start()
    {
        if ( fadeIn )
        {
            FadeIn();

            if ( fadeOut )
            {
                LeanTween.value( 0f, 0f, fadeInTime ).setOnComplete( () => { FadeOut(); } );
            }
        }
        else if ( fadeOut )
        {
            FadeOut();

            if ( destroyOnEnd )
            {
                Destroy( gameObject );
            }
        }
    }


    private void FadeIn()
    {
        if ( fadeInOuterRadius )
        {
            LeanTween.value( minInOuterRadius, maxInOuterRadius, fadeInTime ).setOnUpdate( ( float v ) => { _light.pointLightOuterRadius = v; } );
        }

        if ( fadeInIntensity )
        {
            LeanTween.value( minInIntensity, maxInIntensity, fadeInTime ).setOnUpdate( ( float v ) => { _light.intensity = v; } );
        }
    }


    private void FadeOut()
    {
        if ( fadeOutOuterRadius )
        {
            LeanTween.value( maxOutOuterRadius, minOutOuterRadius, fadeOutTime ).setOnUpdate( ( float v ) => { _light.pointLightOuterRadius = v; } );
        }

        if ( fadeOutIntensity )
        {
            LeanTween.value( maxOutIntensity, minOutIntensity, fadeOutTime ).setOnUpdate( ( float v ) => { _light.intensity = v; } );
        }
    }
}
