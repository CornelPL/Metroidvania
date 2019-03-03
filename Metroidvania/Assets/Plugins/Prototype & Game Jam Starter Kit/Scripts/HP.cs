/**
 * Description: Adds health functionality to a GameObject.
 * Authors: Kornel, Rebel Game Studio
 * Copyright: © 2018-2019 Rebel Game Studio. All rights reserved. For license see: 'LICENSE' file.
 **/

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
	public float CurrentHP { get; private set; }
	public float MaxHP { get { return maxHP; } }

	[Header("External objects")]
	[SerializeField, Tooltip("An optional Slider that acts as a HP Bar.")] private Slider hpBar = null;

	[Header("Tweakable")]
	[SerializeField] private float maxHP = 10;
	[SerializeField] private bool destroyOnNoHP = false;

	[Header("Events")]
	[SerializeField] private UnityEvent onHealthChange = null;
	[SerializeField] private UnityEvent onDeath = null;

	void Start( )
	{
		CurrentHP = maxHP;

		if ( hpBar )
		{
			hpBar.maxValue = maxHP;
			hpBar.value = maxHP;
		}
	}

	/// <summary>
	/// Applies damage to current HP. Respects HP restrictions and fires events if necessary.
	/// </summary>
	/// <param name="damage">Amount of taken damage.</param>
	public void TakeDamage( float damage )
	{
		ChangeHP( -damage );
	}

	/// <summary>
	/// Adds health. Respects HP restrictions and fires events if necessary.
	/// </summary>
	/// <param name="change">Amount of HP change. Negative values for damage, positive for healing.</param>
	public void Heal( float heal )
	{
		ChangeHP( heal );
	}

	/// <summary>
	/// Changes current HP. Respects HP restrictions and fires events if necessary.
	/// </summary>
	/// <param name="change">Amount of HP change. Negative values for damage, positive for healing.</param>
	public void ChangeHP( float change )
	{
		CurrentHP += change;
		CurrentHP = CurrentHP > maxHP ? maxHP : CurrentHP;
		CurrentHP = CurrentHP < 0 ? 0 : CurrentHP;

		onHealthChange.Invoke( );

		if ( hpBar )
			hpBar.value = CurrentHP;

		if ( CurrentHP <= 0 )
			DestroyMe( );
	}

	private void DestroyMe( )
	{
		onDeath.Invoke( );

		if ( destroyOnNoHP )
			Destroy( gameObject );
	}
}
