using UnityEngine;
using MyBox;

public class ExplosionLight : MonoBehaviour
{
    [SerializeField] private UnityEngine.Experimental.Rendering.LWRP.Light2D _light = null;

    [Separator( "Fade In" )]
    [SerializeField] private bool fadeIn = false;
    [SerializeField, ConditionalField( nameof( fadeIn ) )] private float fadeInTime = 1f;

    [SerializeField, ConditionalField( nameof( fadeIn ) )] private bool fadeInOuterRadius = false;
    [SerializeField, ConditionalField( nameof( fadeInOuterRadius ) )] private float minInOuterRadius = 0f;
    [SerializeField, ConditionalField( nameof( fadeInOuterRadius ) )] private float maxInOuterRadius = 10f;

    [SerializeField, ConditionalField( nameof( fadeIn ) )] private bool fadeInIntensity = false;
    [SerializeField, ConditionalField( nameof( fadeInIntensity ) )] private float minInIntensity = 0f;
    [SerializeField, ConditionalField( nameof( fadeInIntensity ) )] private float maxInIntensity = 10f;

    [Separator( "Fade Out" )]
    [SerializeField] private bool fadeOut = false;
    [SerializeField, ConditionalField( nameof( fadeOut ) )] private float fadeOutTime = 1f;

    [SerializeField, ConditionalField( nameof( fadeOut ) )] private bool fadeOutOuterRadius = false;
    [SerializeField, ConditionalField( nameof( fadeOutOuterRadius ) )] private float minOutOuterRadius = 0f;
    [SerializeField, ConditionalField( nameof( fadeOutOuterRadius ) )] private float maxOutOuterRadius = 10f;

    [SerializeField, ConditionalField( nameof( fadeOut ) )] private bool fadeOutIntensity = false;
    [SerializeField, ConditionalField( nameof( fadeOutIntensity ) )] private float minOutIntensity = 0f;
    [SerializeField, ConditionalField( nameof( fadeOutIntensity ) )] private float maxOutIntensity = 10f;

    [Separator( "Bools" )]
    [SerializeField] private bool fadeOnStart = false;
    [SerializeField] private bool destroyOnEnd = false;


    private void Start()
    {
        if ( fadeOnStart )
        {
            DoFade();
        }
    }


    [ButtonMethod]
    public void DoFade()
    {
        if ( fadeIn )
        {
            FadeIn();

            if ( fadeOut )
            {
                LeanTween.value( 0f, 0f, fadeInTime ).setOnComplete( () => { FadeOut(); } );
            }
            else if ( destroyOnEnd )
            {
                LeanTween.value( 0f, 0f, fadeInTime ).setOnComplete( () => { Destroy( gameObject ); } );
            }
        }
        else if ( fadeOut )
        {
            FadeOut();

            if ( destroyOnEnd )
            {
                LeanTween.value( 0f, 0f, fadeOutTime ).setOnComplete( () => { Destroy( gameObject ); } );
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
