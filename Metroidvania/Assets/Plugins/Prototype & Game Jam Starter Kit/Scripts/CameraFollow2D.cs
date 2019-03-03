/**
 * Description: Makes a camera follow a target.
 * Authors: Michał Wildanger, Rebel Game Studio
 * Copyright: © 2018-2019 Rebel Game Studio. All rights reserved. For license see: 'LICENSE' file.
**/

using UnityEngine;
using UnityEngine.Assertions;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target = null;
    [SerializeField] private float smoothSpeed = 0.2f;
    [SerializeField] private Vector3 offset = new Vector3( 0, 0, -10 );

    void Start( )
    {
		if ( target == null ) target = GameObject.FindGameObjectWithTag( "Player" ).transform;
		Assert.IsNotNull( target );

        ScreenShake.Instance?.FollowTarget2D(target);
    }

	void FixedUpdate ( )
	{
		UpdateCameraPosition( );
	}

    /// <summary>
	/// Set a desired target to follow.
	/// </summary>
	/// <param name="newTarget">New target the camera should follow.</param>
	public void ChangeTarget( Transform newTarget )
    {
        target = newTarget;
    }
	private void UpdateCameraPosition( )
	{
		if ( target == null )
			return;

		float desireX = Mathf.SmoothStep( transform.position.x, target.position.x + offset.x, smoothSpeed );
		float desireY = Mathf.SmoothStep( transform.position.y, target.position.y + offset.y, smoothSpeed );

		transform.position = new Vector3( desireX, desireY, offset.z );
	}
}
