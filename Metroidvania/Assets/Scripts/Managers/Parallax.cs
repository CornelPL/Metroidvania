using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Element
{
    public Transform parent;
    public Transform transform;
    public float xSpeedRatio;
    public float ySpeedRatio;
    public Vector3 startPos;

    public Element(Transform p, Transform t, float xs, float ys)
    {
        parent = p;
        transform = t;
        xSpeedRatio = xs;
        ySpeedRatio = ys;
        startPos = t.position;
    }
}


public class Parallax : MonoBehaviour
{
    public static Parallax instance = null;

    [SerializeField] private Transform cam = null;
    [SerializeField] private float speed = 1f;

    private List<Element> elements = new List<Element>();
    private Vector3 lastCameraPosition = Vector3.zero;


    private void Awake()
    {
        if ( instance == null )
            instance = this;
        else if ( instance != this )
            Destroy( this );
    }


    private void Start()
    {
        lastCameraPosition = cam.position;
    }


    private void Update()
    {
        for ( int i = 0; i < elements.Count; i++ )
        {
            Element element = elements[ i ];
            Vector2 distance = lastCameraPosition - cam.position;
            float xOffset = distance.x * element.xSpeedRatio * speed;
            float yOffset = distance.y * element.ySpeedRatio * speed;
            element.transform.position = new Vector3( element.startPos.x + xOffset, element.startPos.y + yOffset, element.startPos.z );
        }
    }


    public void AddElement( Transform element, float xSpeed, float ySpeed )
    {
        elements.Add( new Element( null, element, xSpeed, ySpeed ) );
    }


    public void RemoveElement( Transform element )
    {
        Element e = elements.Find( x => x.transform == element );

        if ( e == null )
        {
            Debug.LogError( "Could not find element " + element.name + " in list" );
            return;
        }
        
        e.transform.position = e.startPos;
        elements.Remove( e );
    }


    public void AddElements( Transform parent, float xSpeed, float ySpeed )
    {
        foreach ( Transform child in parent )
        {
            elements.Add( new Element( parent, child, xSpeed, ySpeed ) );
        }
    }


    public void RemoveElements( Transform parent )
    {
        List<Element> e = elements.FindAll( x => x.parent == parent );
        foreach ( Element element in e )
        {
            element.transform.position = element.startPos;
            elements.Remove( element );
        }
    }
}