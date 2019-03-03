/**
 * Description: Add to your Camera. It allows you trigger ScreenShake using StartScreenShake function.
 * Authors: Michał Wildanger, Rebel Game Studio
 * Copyright: © 2018-2019 Rebel Game Studio.All rights reserved.For license see: 'LICENSE' file.
**/

using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance = null;
    public enum ShakeType { Common, TwoD, ThreeD };

    // Shake
    private ShakeType shakeType = ShakeType.Common;
    private float moveX, moveY, moveZ, rotateX, rotateY, rotateZ;
    private float duration;
    private float timer;
    private bool shaking = false;

    // Recoil
    private Vector2 mainRecoil;
    private Vector2 recoilInMove;
    private Vector2 recoilInRotate;

    // Camera position
    private Transform transformBeforeShake;
    private bool cameraFollowTarget;
    private GameObject cameraCenter;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
    }

    void Update()
    {
        Timer();
        CheckType();
        Shake();
        CheckRotation2D();
    }

    /// <summary>
    /// Sets a target the camera is following. For 2D.
    /// </summary>
    /// <param name="target">Target the camera follows.</param>
    public void FollowTarget2D(Transform target)
    {
        transformBeforeShake = target;
        cameraFollowTarget = true;
    }

	/// <summary>
	/// Sets a target the camera is following. For 3D.
	/// </summary>
	/// <param name="target">Target the camera follows.</param>
	public void FollowTarget3D(Transform target)
    {
        transformBeforeShake = target;
        cameraFollowTarget = true;
    }

	/// <summary>
	/// Starts screen shake with given parameters in 2D.
	/// </summary>
	/// <param name="duration">Duration time of screen shake.</param>
	/// <param name="moveStrength">Strength of the translation.</param>
	/// <param name="rootZStrength">Strength of the rotation (Z axis).</param>
	public void StartScreenShake2D(float duration, float moveStrength, float rootZStrength)
    {
        StartScreenShake(ShakeType.TwoD, duration, new Vector3(1, 1, 1) * moveStrength, new Vector3(0, 0, 1) * rootZStrength);
    }

	/// <summary>
	/// Starts screen shake with given parameters in 3D
	/// </summary>
	/// <param name="duration">Duration time of screen shake.</param>
	/// <param name="strength">Strength of screen shake.</param>
	public void StartScreenShake3D(float duration, float strength)
    {
        StartScreenShake(ShakeType.ThreeD, duration, default(Vector3), new Vector3(1, 1, 1) * strength);
    }

	/// <summary>
	/// Starts a basic screen shake.
	/// </summary>
	/// <param name="duration">Duration time of screen shake.</param>
	/// <param name="moveStrength">Strength of the translation.</param>
	/// <param name="rootStrength">Strength of the rotation (for all axis).</param>
	public void StartScreenShakeBasic(float duration, float moveStrength, float rootStrength)
    {
        StartScreenShake(ShakeType.Common, duration, new Vector3(1, 1, 1) * moveStrength, new Vector3(1, 1, 1) * rootStrength);
    }

	/// <summary>
	/// Recommended to use by CallScreenShake.cs
	/// Starts screen shake.
	/// </summary>
	/// <param name="shakeType">Type of screen shake.</param>
	/// <param name="duration">Duration of screen shake.</param>
	/// <param name="move">Strength of moving camera (skale in x and y).</param>
	/// <param name="rotate">Strength and direction of rotation.</param>
	/// <param name="recoil">Strength and direction of recoil.</param>
	public void StartScreenShake(ShakeType shakeType, float duration, Vector3 move = default(Vector3), Vector3 rotate = default(Vector3), Vector2 recoil = default(Vector2))
    {
        this.shakeType = shakeType;

        if (!shaking && !cameraFollowTarget)
        {
            if (cameraCenter == null)
            {
                cameraCenter = new GameObject("Center of Camera");
                cameraCenter.transform.position = transform.position;
                cameraCenter.transform.rotation = transform.rotation;
                transformBeforeShake = cameraCenter.transform;
            }
            else
            {
                cameraCenter.transform.position = transform.position;
                cameraCenter.transform.rotation = transform.rotation;
            }
        }

        if (move.x > moveX) moveX = move.x;
        if (move.y > moveY) moveY = move.y;
        if (move.z > moveZ) moveZ = move.z;

        if (rotate.x > rotateX) rotateX = rotate.x;
        if (rotate.y > rotateY) rotateY = rotate.y;
        if (rotate.z > rotateZ) rotateZ = rotate.z;

        if (duration > timer)
        {
            timer = duration;
            this.duration = duration;
        }

        mainRecoil = recoil;
        recoilInMove = mainRecoil;
        recoilInRotate = mainRecoil;

        shaking = true;
    }

    private void Shake()
    {
        if (!shaking) return;

        transform.position += new Vector3(RandomBetweenNegAndPos(moveX) + recoilInMove.x,
            RandomBetweenNegAndPos(moveY) + recoilInMove.y,
            RandomBetweenNegAndPos(moveZ)) * Strength();

        transform.eulerAngles += new Vector3(RandomBetweenNegAndPos(rotateX) - recoilInRotate.y,
            RandomBetweenNegAndPos(rotateY) + recoilInRotate.x,
            RandomBetweenNegAndPos(rotateZ)) * Strength();

        if (shakeType != ShakeType.ThreeD || !cameraFollowTarget)
        {
			if ( transform == null || transformBeforeShake == null )
				return;

            transform.position = Vector3.Lerp(transform.position, transformBeforeShake.position, 0.1f);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, 0.1f);
        }
    }

    private void Timer()
    {
        if (!shaking) return;

        if (timer > 0) timer -= Time.deltaTime;
        else EndScreenShake();
    }

    private void EndScreenShake()
    {
        shaking = false;
        moveX = 0;
        moveY = 0;
        moveZ = 0;
        rotateX = 0;
        rotateY = 0;
        rotateZ = 0;
        mainRecoil = recoilInMove = recoilInRotate = new Vector2(0, 0);

        if (!cameraFollowTarget)
        {
            transform.position = transformBeforeShake.position;
            //transform.rotation = transformBeforeShake.rotation;
        }
    }

    private void CheckType()
    {
        if (shakeType == ShakeType.TwoD)
        {
            recoilInRotate = default(Vector3);
            rotateY = 0;
            rotateX = 0;
            moveZ = 0;
        }
        else if (shakeType == ShakeType.ThreeD)
        {
            recoilInMove = default(Vector3);
            moveX = 0;
            moveY = 0;
            moveZ = 0;
        }
    }

    private void CheckRotation2D()
    {
		if ( transform == null || transformBeforeShake == null )
			return;

		if (!cameraFollowTarget || shakeType != ShakeType.TwoD || shaking) return;

        if (transformBeforeShake.rotation != transform.rotation)
        {
            Quaternion currentRotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, 10 * Time.deltaTime);
            transform.rotation = currentRotation;
        }
    }

    private float Strength()
    {
        if (duration != 0)
            return Mathf.Pow(timer / duration, 2);
        else
            return 0;
    }

    private float RandomBetweenNegAndPos(float x)
    {
        x = Mathf.Abs(x);
        return Random.Range(-x, x);
    }
}
