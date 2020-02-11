using UnityEngine;
using MyBox;
using UnityEngine.UI;

public class AutoColor : MonoBehaviour
{
    [Separator( "Fade In" )]
    [SerializeField] private bool fadeIn = false;
    [SerializeField, ConditionalField( nameof( fadeIn ) )] private float fadeInTime = 1f;
    [SerializeField, ConditionalField( nameof( fadeIn ) )] private float minFadeInA = 0f;
    [SerializeField, ConditionalField( nameof( fadeIn ) )] private float maxFadeInA = 1f;

    [Separator( "Fade Out" )]
    [SerializeField] private bool fadeOut = false;
    [ConditionalField( nameof( fadeOut ) )] public float fadeOutTime = 1f;
    [SerializeField, ConditionalField( nameof( fadeOut ) )] private float minFadeOutA = 0f;
    [SerializeField, ConditionalField( nameof( fadeOut ) )] private float maxFadeOutA = 1f;

    [Separator( "Bools" )]
    [SerializeField] private bool fadeOnStart = false;
    [SerializeField] private bool autoFadeOut = false;
    [SerializeField] private bool isImage = false;
    [SerializeField] private bool destroyOnEnd = false;


    private SpriteRenderer _renderer;
    private Image _image;


    private void Start()
    {
        if ( isImage )
        {
            _image = GetComponent<Image>();
        }
        else
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

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
        }
        else if ( fadeOut )
        {
            FadeOut();
        }
    }


    public void FadeIn()
    {
        LTDescr tween;

        if ( isImage )
        {
            Color imageColor = _image.color;

            tween = LeanTween.value( gameObject, minFadeInA, maxFadeInA, fadeInTime )
                .setOnUpdate( ( float v ) => { _image.color = new Color( imageColor.r, imageColor.g, imageColor.b, v ); } );
        }
        else
        {
            Color rendererColor = _renderer.color;

            tween = LeanTween.value( gameObject, minFadeInA, maxFadeInA, fadeInTime )
                .setOnUpdate( ( float v ) => { _renderer.color = new Color( rendererColor.r, rendererColor.g, rendererColor.b, v ); } );
        }

        if ( fadeOut && autoFadeOut )
        {
            tween.setOnComplete( () => { FadeOut(); } );
        }
        else if ( destroyOnEnd )
        {
            tween.setOnComplete( () => { Destroy( gameObject ); } ); 
        }
    }


    public void FadeOut()
    {
        LTDescr tween;

        if ( isImage )
        {
            Color imageColor = _image.color;

            tween = LeanTween.value( gameObject, maxFadeOutA, minFadeOutA, fadeOutTime )
                .setOnUpdate( ( float v ) => { _image.color = new Color( imageColor.r, imageColor.g, imageColor.b, v ); } );
        }
        else
        {
            Color rendererColor = _renderer.color;

            tween = LeanTween.value( gameObject, maxFadeOutA, minFadeOutA, fadeOutTime )
                .setOnUpdate( ( float v ) => { _renderer.color = new Color( rendererColor.r, rendererColor.g, rendererColor.b, v ); } );
        }

        if ( destroyOnEnd )
        {
            tween.setOnComplete( () => { Destroy( gameObject ); } );
        }
    }
}
