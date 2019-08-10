using System.Collections.Generic;
using UnityEngine;


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