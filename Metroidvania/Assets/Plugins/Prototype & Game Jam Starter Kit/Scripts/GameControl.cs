/**
 * Description: Allows to exit the game and/or reset the current scene.
 * Authors: Kornel, Rebel Game Studio
 * Copyright: © 2018-2019 Rebel Game Studio. All rights reserved. For license see: 'LICENSE' file.
 **/

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
	[SerializeField] private bool allowExit = true;
	[SerializeField] private bool allowReset = true;

	void Update( )
	{
		if ( allowReset && Input.GetKeyDown( KeyCode.R ) )
			SceneManager.LoadScene( gameObject.scene.name );

		if ( allowExit && Input.GetKeyDown( KeyCode.Escape ) )
			Application.Quit( );

#if UNITY_EDITOR
		if ( allowExit && Input.GetKeyDown( KeyCode.Escape ) )
			UnityEditor.EditorApplication.isPlaying = false;
#endif
	}
}
