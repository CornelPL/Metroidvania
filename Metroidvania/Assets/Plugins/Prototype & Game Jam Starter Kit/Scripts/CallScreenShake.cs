/**
 * Description: Helper class for calling screen shake from the inspector.
 * Authors: Michał Wildanger, Rebel Game Studio
 * Copyright: © 2018-2019 Rebel Game Studio. All rights reserved. For license see: 'LICENSE' file.
**/

using UnityEngine;

public class CallScreenShake : MonoBehaviour
{
    public enum EditionType { Basic, Advanced };

    public EditionType Edition = EditionType.Basic;
    public ScreenShake.ShakeType ChosedShakeType = ScreenShake.ShakeType.Common;

    public Vector3 Move = new Vector3(1, 1, 1) * 0.3f;
    public Vector3 Rotation = new Vector3(1, 1, 3);
    public Vector2 RecoilDirection = new Vector2(0, 0);

    public float RecoilStrength = 0;
    public float GeneralStrength = 0.3f;
    public float Duration = 1;

	/// <summary>
	/// Starts screen shake with parameters assigned in the inspector.
	/// </summary>
	public void StartScreenShake()
    {
        ScreenShake.Instance.StartScreenShake(ChosedShakeType, Duration, Move, Rotation, RecoilDirection * RecoilStrength);
    }

	/// <summary>
	/// Sets screen shake parameters to default and multiplies them by the strength parameter.
	/// </summary>
	/// <param name="strength">Multiplier of the default values.</param>
	public void BasicSettings(float strength)
    {
        Move = new Vector3(1, 1, 1) * strength;
        Rotation = new Vector3(1, 1, 1) * strength;
    }
}
