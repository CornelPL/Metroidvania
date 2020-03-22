using UnityEngine;
using MyBox;

public class TelekinesisEffects : MonoBehaviour
{
    [SerializeField, MustBeAssigned] private ParticleSystem onOverItemParticles = null;
    [SerializeField, MustBeAssigned] private GameObject innerSurfaceHighlight = null;
    [SerializeField, MustBeAssigned] private GameObject outerSurfaceHighlight = null;

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

        if ( on )
            onOverItemParticles.transform.position = closestItem.transform.position;

        if ( !on && onOverItemParticles.isPlaying )
        {
            onOverItemParticles.Stop();
            onOverItemParticles.Clear();
        }
        else if ( on && !onOverItemParticles.isPlaying )
        {
            onOverItemParticles.Play();
        }
    }


    public void SetInnerHighlight( bool on )
    {
        isInnerHighlightActive = on;

        if ( on )
        {
            innerSurfaceHighlight.transform.position = input.cursorPosition;
            onOverItemParticles.transform.position = input.cursorPosition;
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


    private void Start()
    {
        input = InputController.instance;
    }


    private void Update()
    {
        if ( areOverItemEffectsActive )
        {
            onOverItemParticles.transform.position = closestItem.transform.position;
        }
        else if ( isInnerHighlightActive )
        {
            innerSurfaceHighlight.transform.position = input.cursorPosition;
            onOverItemParticles.transform.position = input.cursorPosition;
        }
        else if ( isOuterHighlightActive )
        {
            outerSurfaceHighlight.transform.position = closestPoint;
        }
    }
}
