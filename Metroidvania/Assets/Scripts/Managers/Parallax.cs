using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ElementsGroup
{
    public GameObject parent;
    public float speedRatio = 0f;
}


public class Element
{
    public Transform transform;
    public float speedRatio;
    public Vector3 startPos;

    public Element(Transform t, float s, Vector3 v)
    {
        transform = t;
        speedRatio = s;
        startPos = v;
    }
}


public class Parallax : MonoBehaviour
{
    [SerializeField] private Transform cam = null;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float maxDistance = 30f;
    [SerializeField] private float maxOffset = 2f;
    public List<ElementsGroup> parallaxObjectsGroups;

    private List<Element> elements = new List<Element>();

    private void Start()
    {
        for(int i = 0; i < parallaxObjectsGroups.Count; i++)
        {
            foreach(Transform child in parallaxObjectsGroups[i].parent.transform)
            {
                float parentsSpeedRatio = parallaxObjectsGroups[i].speedRatio;
                elements.Add(new Element(child, parentsSpeedRatio, child.position));
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < elements.Count; i++)
        {
            Element element = elements[i];
            float distance = Vector2.Distance(cam.position, element.startPos);
            if (distance < maxDistance)
            {
                float xDistance = element.startPos.x - cam.position.x;
                float sign = Mathf.Sign(element.startPos.x - cam.position.x);
                xDistance = Mathf.Abs(xDistance);
                float o = xDistance * element.speedRatio * speed;
                float offset = o < maxOffset ? o : maxOffset;
                offset *= sign;
                element.transform.position = new Vector3(element.startPos.x + offset, element.startPos.y, element.startPos.z);
            }
        }
    }
}
