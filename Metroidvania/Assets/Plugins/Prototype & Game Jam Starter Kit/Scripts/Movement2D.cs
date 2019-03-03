/**
 * Description: Movement script for top-down and platformer 2D games.
 * Authors: Paweł, Rebel Game Studio
 * Copyright: © 2018-2019 Rebel Game Studio. All rights reserved. For license see: 'LICENSE' file.
 * */

using UnityEngine;

public class Movement2D : MonoBehaviour
{
    private enum Type { TopDown, Platformer };

    [Header("Tweakables")]
    [SerializeField] private Type gameType = Type.Platformer;
    [SerializeField] private string groundTag = "Ground";
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 7f;

    [Header("Controls")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    private Rigidbody2D rb;
    private bool isJumping;
    private float forceX, forceY;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
	{
		DoMovement( );
	}

	void OnCollisionEnter2D( Collision2D collision )
	{
		if ( collision.gameObject.CompareTag( groundTag ) )
			isJumping = false;
	}

	private void DoMovement( )
	{
		if ( gameType == Type.TopDown )
		{
			float horizontalInput = Input.GetAxisRaw( "Horizontal" );
			float verticalInput = Input.GetAxisRaw( "Vertical" );

			float newX = transform.position.x + horizontalInput * speed * Time.deltaTime;
			float newY = transform.position.y + verticalInput * speed * Time.deltaTime;

			transform.position = Vector2.Lerp( transform.position, new Vector2( newX, newY ), 1f );
		}
		else if ( gameType == Type.Platformer )
		{
			float horizontalInput = Input.GetAxisRaw( "Horizontal" );

			float newX = transform.position.x + horizontalInput * speed * Time.deltaTime;

			if ( Input.GetKey( jumpKey ) && !isJumping )
			{
				rb.AddForce( new Vector2( 0, jumpForce ) );
				isJumping = true;
			}

			transform.position = Vector2.Lerp( transform.position, new Vector2( newX, transform.position.y ), 1f );
		}
	}
}
