/**
 * Description: Sets object enabled or disabled on start.
 * Authors: Kornel, Rebel Game Studio
 * Copyright: © 2018-2019 Rebel Game Studio. All rights reserved. For license see: 'LICENSE' file.
 **/

using UnityEngine;

public class SetEnabled : MonoBehaviour
{
	[SerializeField, Tooltip("Objects to enable")] private GameObject[] toEnable = null;
	[SerializeField] private bool enable = true;

	void Start( )
	{
		foreach ( var item in toEnable )
			item.SetActive( enable );
	}
}
