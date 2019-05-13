using System.Collections.Generic;
using UnityEngine;


public class ElementsGroup
{
    public GameObject parent;
    public float speedRatio = 0f;
}


public class Element
{
    private Transform transform;
    private float speedRatio;
}


public class Parallax : MonoBehaviour
{
    [SerializeField] private Transform player = null;
    [SerializeField] private List<ElementsGroup> parallaxObjectsGroups = new List<ElementsGroup>();

    private List<Element> elements = new List<Element>();

    void Start()
    {
        for(int i = 0; i < parallaxObjectsGroups.Count; i++)
        {
            //parallaxObjectsGroups[i].parent.
        }
    }

    void Update()
    {
        
    }
}
