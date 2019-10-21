/**
 * Description: Custom cursor with added interactivity.
 * Authors: Kornel
 * Copyright: © 2019 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class CustomCursor : MonoBehaviour
{
	public static CustomCursor Instance { get; private set; }

	[Header("Cursor Images")]
	[SerializeField] private Image outerImage = null;
	[SerializeField] private Image innerImage = null;

	[Header("Colors")]
	[SerializeField] private Color normalColorOuter = Color.white;
	[SerializeField] private Color normalColorInner = Color.black;
	[SerializeField] private Color clickInsideColor = Color.red;
	[SerializeField] private Color overColor = Color.yellow;

	[Header("Parameters")]
	[SerializeField, Tooltip("Works on any UI element that blocks raycasts")] private bool highlighOnOverUI = true;
	[SerializeField] private bool useInsideImage = true;
	[SerializeField] private float onClickScale = 2f;
	[SerializeField] private float scaleDecreaseOverTime = 6f;
	[SerializeField] private float minScale = 1f;
	[SerializeField] private Vector3 offset = Vector3.zero;

	private float currentScale = 1f;

	private void OnEnable( )
	{
		if ( Instance != null && Instance != this )
			Destroy( this );
		else
			Instance = this;
	}

	private void OnDestroy( ) { if ( this == Instance ) { Instance = null; } }
	private void OnDisable( ) { if ( this == Instance ) { Instance = null; } }

	private void Start( )
	{
		Assert.IsNotNull( outerImage, $"Please assign <b>{nameof( outerImage )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );
		if ( useInsideImage )
			Assert.IsNotNull( innerImage, $"Please assign <b>{nameof( innerImage )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		if ( useInsideImage )
			innerImage.color = normalColorInner;
		currentScale = minScale;

		CanvasGroup cg = GetComponent<CanvasGroup>( );
		cg.interactable = false;
		cg.blocksRaycasts = false;

		Cursor.visible = false;
	}

	public void OnInteraction( )
	{
		currentScale = onClickScale;
	}

	private void Update( )
	{
		UpdatePosition( );
		UpdateVisualState( );
	}

	private void UpdatePosition( )
	{
		transform.position = Input.mousePosition + offset;
	}

	private void UpdateVisualState( )
	{
		currentScale -= scaleDecreaseOverTime * Time.deltaTime;
		currentScale = Mathf.Clamp( currentScale, minScale, onClickScale );

		transform.localScale = Vector3.one * currentScale;

		if ( useInsideImage )
		{
			if ( Input.GetMouseButtonDown( 0 ) )
				innerImage.color = clickInsideColor;
			else if ( Input.GetMouseButtonUp( 0 ) )
				innerImage.color = normalColorInner;
		}

		if ( highlighOnOverUI && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject( ) )
			outerImage.color = overColor;
		else
			outerImage.color = normalColorOuter;
	}
}
