using UnityEngine;
using MyBox;
using UnityEngine.UI;
using UnityEngine.U2D;

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
    [SerializeField] private bool isSpriteShape = false;
    [SerializeField] private bool destroyOnEnd = false;


    private SpriteRenderer SRenderer;
    private SpriteShapeRenderer SSRenderer;
    private Image _image;


    private void Start()
    {
        if ( isImage )
        {
            _image = GetComponent<Image>();
        }
        else if ( isSpriteShape )
        {
            SSRenderer = GetComponent<SpriteShapeRenderer>();
        }
        else
        {
            SRenderer = GetComponent<SpriteRenderer>();
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
        else if ( isSpriteShape )
        {
            Color rendererColor = SSRenderer.color;

            tween = LeanTween.value( gameObject, minFadeInA, maxFadeInA, fadeInTime )
                .setOnUpdate( ( float v ) => { SSRenderer.color = new Color( rendererColor.r, rendererColor.g, rendererColor.b, v ); } );
        }
        else
        {
            Color rendererColor = SRenderer.color;

            tween = LeanTween.value( gameObject, minFadeInA, maxFadeInA, fadeInTime )
                .setOnUpdate( ( float v ) => { SRenderer.color = new Color( rendererColor.r, rendererColor.g, rendererColor.b, v ); } );
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
        else if ( isSpriteShape )
        {
            Color rendererColor = SSRenderer.color;

            tween = LeanTween.value( gameObject, maxFadeOutA, minFadeOutA, fadeOutTime )
                .setOnUpdate( ( float v ) => { SSRenderer.color = new Color( rendererColor.r, rendererColor.g, rendererColor.b, v ); } );
        }
        else
        {
            Color rendererColor = SRenderer.color;

            tween = LeanTween.value( gameObject, maxFadeOutA, minFadeOutA, fadeOutTime )
                .setOnUpdate( ( float v ) => { SRenderer.color = new Color( rendererColor.r, rendererColor.g, rendererColor.b, v ); } );
        }

        if ( destroyOnEnd )
        {
            tween.setOnComplete( () => { Destroy( gameObject ); } );
        }
    }
}
