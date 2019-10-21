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
	[SerializeField] private Image cursorImage = null;

	[Header("Colors")]
	[SerializeField] private Color normalColor = Color.white;
	[SerializeField] private Color clickColor = Color.red;
	[SerializeField] private Color overColor = Color.yellow;
	[SerializeField] private Color inRangeColor = Color.white;

	[Header("Parameters")]
	[SerializeField, Tooltip("Works on any UI element that blocks raycasts")] private bool highlighOnOverUI = true;
	[SerializeField] private float onClickScale = 2f;
	[SerializeField] private float scaleDecreaseOverTime = 6f;
	[SerializeField] private float minScale = 1f;
	[SerializeField] private Vector3 offset = Vector3.zero;


	private float currentScale = 1f;
    private bool inRange = false;
    private bool isOver;


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
		Assert.IsNotNull( cursorImage, $"Please assign <b>{nameof( cursorImage )}</b> field on <b>{GetType( ).Name}</b> script on <b>{name}</b> object" );

		currentScale = minScale;

		CanvasGroup cg = GetComponent<CanvasGroup>( );
		cg.interactable = false;
		cg.blocksRaycasts = false;

		Cursor.visible = false;
	}


    public void OnInRangeChange( bool inRange )
    {
        cursorImage.color = inRange ? inRangeColor : normalColor;
        this.inRange = inRange;
    }


    public void OnOverChange( bool isOver )
    {
        cursorImage.color = isOver ? overColor : normalColor;
        this.isOver = isOver;
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

		if ( highlighOnOverUI && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject( ) )
			cursorImage.color = overColor;
		else if ( !inRange && !isOver)
			cursorImage.color = normalColor;
	}
}
