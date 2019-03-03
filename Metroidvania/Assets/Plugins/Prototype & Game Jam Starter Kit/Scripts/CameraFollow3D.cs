/**
 * Description: Makes a camera follow a target.
 * Authors: Michał Wildanger, Rebel Game Studio
 * Copyright: © 2018-2019 Rebel Game Studio. All rights reserved. For license see: 'LICENSE' file.
**/

using UnityEngine;
using UnityEngine.Assertions;

public class CameraFollow3D : MonoBehaviour
{
    [SerializeField] private Transform target = null;
    [SerializeField] private Vector3 distance = new Vector3(0,0,10);
    [SerializeField] private float moveTimeToReceive = 3;
    [SerializeField] private float rootTimeToReceive = 3;

    private Transform cameraHandler = null;

    void Start()
    {
        if (target == null) target = GameObject.FindGameObjectWithTag("Player").transform;
        Assert.IsNotNull(target);

        if (ScreenShake.Instance != null)
        {
            cameraHandler = new GameObject("Camera Handle").transform;
            ScreenShake.Instance.FollowTarget3D(target);
        }
    }

    void FixedUpdate ()
	{
		UpdatePositionAndRotation( );
	}

	/// <summary>
	/// Set a desired target to follow.
	/// </summary>
	/// <param name="newTarget">New target the camera should follow.</param>
	public void ChangeTarget(Transform newTarget)
    {
        target = newTarget;
    }

	private void UpdatePositionAndRotation( )
	{
		Vector3 desiredPosition = target.position - ( target.rotation * distance );
		Vector3 currentPosition = Vector3.Lerp( transform.position, desiredPosition, moveTimeToReceive * Time.deltaTime );
		transform.position = currentPosition;

		Quaternion desiredRotation = Quaternion.LookRotation( target.position - transform.position, target.up );
		Quaternion currentRotation = Quaternion.Slerp( transform.rotation, desiredRotation, rootTimeToReceive * Time.deltaTime );
		transform.rotation = currentRotation;

		cameraHandler.position = transform.position;
		cameraHandler.rotation = transform.rotation;
	}
}
