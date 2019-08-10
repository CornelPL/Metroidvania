using System.Collections.Generic;
using UnityEngine;


public class Element
{
    public Transform transform;
    public float xSpeedRatio;
    public float ySpeedRatio;
    public Vector3 startPos;

    public Element(Transform t, float xs, float ys)
    {
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


    private void Awake()
    {
        if ( instance == null )
            instance = this;
        else if ( instance != this )
            Destroy( this );
    }


    private void Update()
    {
        for ( int i = 0; i < elements.Count; i++ )
        {
            Element element = elements[ i ];
            Vector2 distance = element.startPos - cam.position;
            float xSign = -Mathf.Sign( distance.x );
            float ySign = -Mathf.Sign( distance.y );
            float xOffset = Mathf.Abs( distance.x ) * element.xSpeedRatio * speed * xSign;
            float yOffset = Mathf.Abs( distance.y ) * element.ySpeedRatio * speed * ySign;
            element.transform.position = new Vector3( element.startPos.x + xOffset, element.startPos.y + yOffset, element.startPos.z );
        }
    }


    public void AddElement( Transform elementToAdd, float xSpeed, float ySpeed )
    {
        elements.Add( new Element( elementToAdd, xSpeed, ySpeed ) );
    }


    public void RemoveElement( Transform elementToRemove )
    {
        Element e = elements.Find( x => x.transform == elementToRemove );
        e.transform.position = e.startPos;

        elements.Remove( e );
    }
}