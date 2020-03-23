using UnityEngine;
using MyBox;

public class TelekinesisEffects : MonoBehaviour
{
    [SerializeField, MustBeAssigned] private ParticleSystem onOverItemParticles = null;
    [SerializeField, MustBeAssigned] private GameObject innerSurfaceHighlight = null;
    [SerializeField, MustBeAssigned] private GameObject outerSurfaceHighlight = null;
    [SerializeField, MustBeAssigned] private GameObject pullEffects = null;
    [SerializeField, MustBeAssigned] private ExplosionLight eyesLight = null;

    [HideInInspector] public bool isCursorOver = false;
    [HideInInspector] public bool areOverItemEffectsActive = false;
    [HideInInspector] public bool isInnerHighlightActive = false;
    [HideInInspector] public bool isOuterHighlightActive = false;
    [HideInInspector] public Transform closestItem = null;
    [HideInInspector] public Vector2 closestPoint = Vector2.zero;

    private InputController input = null;


    public void SetCursorOver( bool on )
    {
        isCursorOver = on;
        CustomCursor.Instance?.SetOver( on );
    }


    public void SetOverItemEffects( bool on )
    {
        areOverItemEffectsActive = on;
        onOverItemParticles.gameObject.SetActive( on );

        if ( on )
            onOverItemParticles.transform.position = closestItem.position;
    }


    public void SetInnerHighlight( bool on )
    {
        isInnerHighlightActive = on;

        if ( on )
        {
            innerSurfaceHighlight.transform.position = input.cursorPosition;
        }

        innerSurfaceHighlight.SetActive( on );
    }


    public void SetOuterHighlight( bool on )
    {
        isOuterHighlightActive = on;

        if ( on )
        {
            outerSurfaceHighlight.transform.position = closestPoint;
        }

        outerSurfaceHighlight.SetActive( on );
    }


    public void SetPullItemEffects( bool on )
    {
        pullEffects.SetActive( on );

        if ( on )
        {
            eyesLight.FadeIn();
        }
        else
        {
            eyesLight.FadeOut();
        }
    }


    private void Start()
    {
        input = InputController.instance;
    }


    private void Update()
    {
        if ( isInnerHighlightActive )
        {
            innerSurfaceHighlight.transform.position = input.cursorPosition;
        }
        else if ( isOuterHighlightActive )
        {
            outerSurfaceHighlight.transform.position = closestPoint;
        }
        else if ( areOverItemEffectsActive)
        {
            onOverItemParticles.transform.position = closestItem.transform.position;
        }
    }
}
