using UnityEngine;
using MyBox;
using UnityEngine.Events;

public class AutoScale : MonoBehaviour
{
    [Separator( "Scale Up" )]
    [SerializeField] private bool scaleUp = false;
    [SerializeField, ConditionalField( nameof( scaleUp ) )] private float scaleUpTime = 1f;
    [SerializeField, ConditionalField( nameof( scaleUp ) )] private Vector3 minUpScale = Vector3.zero;
    [SerializeField, ConditionalField( nameof( scaleUp ) )] private Vector3 maxUpScale = Vector3.one;
    [SerializeField] private UnityEvent OnScaledUp = null;

    [Separator( "Scale Down" )]
    [SerializeField] private bool scaleDown = false;
    [SerializeField, ConditionalField( nameof( scaleDown ) )] private float scaleDownTime = 1f;
    [SerializeField, ConditionalField( nameof( scaleDown ) )] private Vector3 minDownScale = Vector3.zero;
    [SerializeField, ConditionalField( nameof( scaleDown ) )] private Vector3 maxDownScale = Vector3.one;
    [SerializeField] private UnityEvent OnScaledDown = null;

    [Separator( "Bools" )]
    [SerializeField] private bool scaleOnStart = false;
    [SerializeField] private bool autoScaleDown = false;
    [SerializeField] private bool useUnscaledTime = false;
    [SerializeField] private bool destroyOnEnd = false;

    private int tweenID = 0;


    private void Start()
    {
        if ( scaleOnStart )
        {
            DoScale();
        }
    }


    [ButtonMethod]
    public void DoScale()
    {
        if ( scaleUp )
        {
            ScaleUp();
        }
        else if ( scaleDown )
        {
            ScaleDown();
        }
    }


    public void ScaleUp()
    {
        if ( tweenID != 0 && LeanTween.isTweening( tweenID ) )
        {
            LeanTween.cancel( tweenID );
        }

        LTDescr tween = LeanTween.value( gameObject, minUpScale, maxUpScale, scaleUpTime ).setOnUpdate( ( Vector3 v ) => { transform.localScale = v; } ).setOnComplete( () => OnScaledUp.Invoke() );

        tweenID = tween.uniqueId;

        if ( useUnscaledTime )
        {
            tween.setIgnoreTimeScale( true );
        }

        if ( scaleDown && autoScaleDown )
        {
            tween.setOnComplete( () => { ScaleDown(); } );
        }
        else if ( !scaleDown && destroyOnEnd )
        {
            tween.setOnComplete( () => { Destroy( gameObject ); } ); 
        }
    }


    public void ScaleDown()
    {
        if ( tweenID != 0 && LeanTween.isTweening( tweenID ) )
        {
            LeanTween.cancel( tweenID );
        }

        LTDescr tween = LeanTween.value( gameObject, maxDownScale, minDownScale, scaleDownTime ).setOnUpdate( ( Vector3 v ) => { transform.localScale = v; } ).setOnComplete( () => OnScaledDown.Invoke() );

        tweenID = tween.uniqueId;

        if ( useUnscaledTime )
        {
            tween.setIgnoreTimeScale( true );
        }

        if ( destroyOnEnd )
        {
            tween.setOnComplete( () => { Destroy( gameObject ); } );
        }
    }
}
