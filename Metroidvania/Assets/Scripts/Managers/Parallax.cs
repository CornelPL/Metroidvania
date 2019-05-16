using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ElementsGroup
{
    public GameObject parent;
    public float xSpeedRatio = 0f;
    public float ySpeedRatio = 0f;
}


public class Element
{
    public Transform transform;
    public float xSpeedRatio;
    public float ySpeedRatio;
    public Vector3 startPos;

    public Element(Transform t, float xs, float ys, Vector3 v)
    {
        transform = t;
        xSpeedRatio = xs;
        ySpeedRatio = ys;
        startPos = v;
    }
}


public class Parallax : MonoBehaviour
{
    [SerializeField] private Transform cam = null;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float maxDistance = 30f;
    public List<ElementsGroup> parallaxObjectsGroups;

    private List<Element> elements = new List<Element>();

    private void Start()
    {
        for(int i = 0; i < parallaxObjectsGroups.Count; i++)
        {
            foreach(Transform child in parallaxObjectsGroups[i].parent.transform)
            {
                ElementsGroup parent = parallaxObjectsGroups[i];
                elements.Add(new Element(child, parent.xSpeedRatio, parent.ySpeedRatio, child.position));
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < elements.Count; i++)
        {
            Element element = elements[i];
            Vector2 distance = element.startPos - cam.position;
            if (distance.magnitude < maxDistance)
            {
                float xSign = -Mathf.Sign(distance.x);
                float ySign = -Mathf.Sign(distance.y);
                float xOffset = Mathf.Abs(distance.x) * element.xSpeedRatio * speed * xSign;
                float yOffset = Mathf.Abs(distance.y) * element.ySpeedRatio * speed * ySign;
                element.transform.position = new Vector3(element.startPos.x + xOffset, element.startPos.y + yOffset, element.startPos.z);
            }
        }
    }
}
