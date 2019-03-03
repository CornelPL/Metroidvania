/**
 * Description: Blinks sprites, for example in response to taken damage.
 * Authors: Kornel, Rebel Game Studio
 * Copyright: © 2018-2019 Rebel Game Studio. All rights reserved. For license see: 'LICENSE' file.
 **/

using UnityEngine;
using UnityEngine.Assertions;

enum BlinkMode
{
	Change,
	Hide
}

public class SpritesBlink : MonoBehaviour
{
	[Header("External objects")]
	[SerializeField] private SpriteRenderer[] spritesToBlink = null;
	[SerializeField] private Material normalMaterial = null;
	[SerializeField] private Material blinkMaterial = null;
	[SerializeField] private BlinkMode mode = BlinkMode.Change;

	[Header("Tweakable")]
	[SerializeField] private float blinkTime = 0.1f;

	void Start ()
	{
		if ( spritesToBlink.Length == 0 )
			spritesToBlink = GetComponentsInChildren<SpriteRenderer>( );

		Assert.IsNotNull( spritesToBlink, "You need to add sprites." );
		Assert.AreNotEqual( spritesToBlink.Length, 0, "You need to add sprites." );

		if ( mode == BlinkMode.Change )
		{
			Assert.IsNotNull( normalMaterial, "Normal material can not be empty." );
			Assert.IsNotNull( blinkMaterial, "Blink material can not be empty." );
		}
	}

	/// <summary>
	/// Do a blink.
	/// </summary>
	public void Blink()
	{
		Blink( blinkTime );
	}

	/// <summary>
	/// Do a blink and overwrite the blink time from the inspector.
	/// </summary>
	/// <param name="time">Time the blink will last.</param>
	public void Blink( float time )
	{
		if ( mode == BlinkMode.Change )
			SwapMaterial( blinkMaterial );
		else
			Hide( true );

		Invoke( "Unblink", time );
	}

	private void Unblink( )
	{
		if ( mode == BlinkMode.Change )
			SwapMaterial( normalMaterial );
		else
			Hide( false );
	}

	private void SwapMaterial( Material material )
	{
		foreach ( var sprite in spritesToBlink )
			sprite.material = material;
	}

	private void Hide( bool state )
	{
		foreach ( var sprite in spritesToBlink )
			sprite.enabled = !state;
	}
}
