/**
 * Description: Detects object in range or if enters on trigger
 * Authors: Wojciech Bruski, Rebel Game Studio
 * Copyright: © 2018-2019 Rebel Game Studio. All rights reserved. For license see: 'LICENSE' file.
 **/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;

[System.Serializable] public class GameObjectListUnityEvent : UnityEvent<List<GameObject>> { }
[System.Serializable] public class GameObjectUnityEvent : UnityEvent<GameObject> { }

public class DetectObjectInRange : MonoBehaviour
{
    public enum DetectionType { Distance, Trigger }

    public enum InvokeDelay { EveryFrame, Time }

    [SerializeField] private DetectionType detectionType = DetectionType.Distance;
    [SerializeField] private InvokeDelay invokeDelay = InvokeDelay.EveryFrame;

    [SerializeField] private float rangeOfDetection = 1.0f;
    [SerializeField] private float invokeDelayTime = 1.0f;

    [SerializeField] private bool anyTag = false;
    [SerializeField] private List<string> detectionTags = new List<string>();

    [SerializeField] private GameObjectListUnityEvent objectsInRangeWithParams = null;
    [SerializeField] private UnityEvent onObjectsInRange = null;

    [SerializeField] private GameObjectUnityEvent objectOutOfRangeWithParams = null;
    [SerializeField] private UnityEvent onObjectOutOfRange = null;

    private List<GameObject> objectsInRange = new List<GameObject>();
    private IEnumerator invokeCoroutine;
    private bool isInvoking = false;

    private void Awake()
    {
        invokeCoroutine = InvokeEventInTime();

        if (detectionType == DetectionType.Trigger)
        {
            Assert.IsNotNull(GetComponent<Collider2D>(), "Missing component - Collider2D");
        }
    }

    private void Update()
    {
        if (invokeDelay == InvokeDelay.EveryFrame)
        {
            InvokeEventInRange();
        }
    }

    private void FixedUpdate()
    {
        if (detectionType == DetectionType.Distance)
		{
			DetectObjectsInDistance( );
		}
	}

	#region CollisionDetection

	private void OnTriggerEnter2D(Collider2D collision)
    {
        if (detectionType == DetectionType.Trigger)
        {
            if (!anyTag)
            {
                foreach (string tag in detectionTags)
                {
                    if (collision.CompareTag(tag))
                    {
                        objectsInRange.Add(collision.gameObject);
                    }
                }
            }
            else
            {
                objectsInRange.Add(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (objectsInRange.Contains(collision.gameObject))
        {
            objectsInRange.Remove(collision.gameObject);

            objectOutOfRangeWithParams.Invoke(collision.gameObject);
            onObjectOutOfRange.Invoke();
        }
    }

	#endregion CollisionDetection

	private void DetectObjectsInDistance( )
	{
		List<GameObject> objs = new List<GameObject>( );

		if ( !anyTag )
		{
			foreach ( string tag in detectionTags )
			{
				List<GameObject> objects = GameObject.FindGameObjectsWithTag( tag ).ToList( );
				objects = objects.Where( o => Vector2.Distance( transform.position, o.transform.position ) <= rangeOfDetection ).ToList( );

				foreach ( GameObject obj in objects )
				{
					objs.Add( obj );
				}
			}
		}
		else
		{
			objs = FindObjectsOfType<GameObject>( ).Where( o => Vector2.Distance( transform.position, o.transform.position ) <= rangeOfDetection ).ToList( );
		}

		foreach ( GameObject obj in objectsInRange )
		{
			if ( !objs.Contains( obj ) )
			{
				objectOutOfRangeWithParams.Invoke( obj );
				onObjectOutOfRange.Invoke( );
			}
		}

		objectsInRange = objs;
	}

	private IEnumerator InvokeEventInTime()
    {
        while (true)
        {
            InvokeEventInRange();
            yield return new WaitForSeconds(invokeDelayTime);
        }
    }

    private void InvokeEventInRange()
    {
        if (objectsInRange.Count > 0)
        {
            onObjectsInRange.Invoke();
            objectsInRangeWithParams.Invoke(objectsInRange);

            if (invokeDelay == InvokeDelay.Time)
            {
                if (!isInvoking)
                {
                    StartCoroutine(invokeCoroutine);
                    isInvoking = true;
                }
            }
        }
        else
        {
            if (invokeDelay == InvokeDelay.Time)
            {
                if (isInvoking)
                {
                    StopCoroutine(invokeCoroutine);
                    isInvoking = false;
                }
            }
        }
    }
}
