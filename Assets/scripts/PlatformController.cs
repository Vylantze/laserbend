﻿/*
 * 
 * 
 * Uses direct velocity manipulation
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
*/

using UnityEngine;
using System.Collections;

public class PlatformController : PlayerController{
	public bool jump = false;
	public bool facingRight = true;
	public float moveForce = 365f;
	public float maxHoriSpeed = 10f;
	public float maxVerticalSpeed = 20f;
	public float jumpForce = 1000f;
	public GameObject female_chara;
	public GameObject male_chara;
	public Animator charge;

	private MasterPlayer master;
	private Rigidbody2D rb2d;
	Collider2D[] colliders;

	// movement
	private bool allowJump = false;

	// Use this for initialization
	void Start () {
		master = MasterPlayer.mainPlayer;
		colliders = GetComponentsInChildren<Collider2D>();
		rb2d = GetComponentInParent<Rigidbody2D> ();
		//anim = GetComponentInChildren<Animator> ();
	}

	// Update is called once per frame
	void Update () {
		// Jump
		keyboard();
		if (Input.GetButtonDown("Jump")) {
			jump = true;
		}
		movement (master.shipMode);
		movementPlatform ();
		Flip ();
	}

	void Flip() {
		if (master.flip!= facingRight) {
			master.flip = facingRight;
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}
	}

	public void setAnimator(Animator value) {
		anim = value;
	}

	
	void OnCollisionStay2D(Collision2D collision) {
		if (collision.collider.CompareTag("ground") ){
			ContactPoint2D[] contacts = collision.contacts;
			Vector2 point = contacts[contacts.Length/2].point;

			// as long as point is below the character's collider
			if ((point.y <transform.position.y-0.69f))// basically, height of chara is 1.4
			    //&&(Mathf.Abs(point.x-transform.position.x)>0.2) )   // and width of chara is 0.4
			    //&&((point.x <transform.position.x-0.2f)||(point.x >transform.position.x+0.2f)))
				{
				allowJump = true;
				anim.SetBool ("in_air", false);
				master.in_air = false;
			}
			else {
				allowJump = false;
				anim.SetBool ("in_air", true);
				master.in_air = true;
			}
		}
	}

	void OnCollisionExit2D(Collision2D collision) {
		allowJump = false;
		anim.SetBool ("in_air", true);
		master.in_air = true;
	}

	public void movementPlatform() { // can access stage directly, but will receive error if not on stage
		// put limiters on speed here
		if (xvel > maxHoriSpeed) { xvel = (int)maxHoriSpeed; } 
		else if (xvel < -maxHoriSpeed) { xvel = (int)-maxHoriSpeed; }
		float y_vel = rb2d.velocity.y; // ignore input for yvel

		//if (jump && yvel == 0f) {
		//	rb2d.AddForce (new Vector2 (0f, jumpForce));
		/*// Old code
		if (jump&&!master.shipMode) {
			foreach(Collider2D collider in colliders) {
				if (collider.IsTouchingLayers(LayerMask.GetMask("Ground"))&&Mathf.Abs(yvel)<=0.3f) {
					rb2d.AddForce (new Vector2 (0f, jumpForce));
					master.SetTrigger("jump");
					break;
				}
			}
		} else {
		}//*/

		/* // old version
		anim.SetBool ("in_air", true);
		master.in_air = true;
		foreach(Collider2D collider in colliders) {
			if (collider.IsTouchingLayers(LayerMask.GetMask("Ground"))&&Mathf.Abs(yvel)<=0.3f) {
				anim.SetBool ("in_air", false);
				master.in_air = false;
				break;
			}
		}//*/

		if (jump && allowJump) {//!master.in_air) {
			y_vel = 0;
			rb2d.AddForce (new Vector2 (0f, jumpForce));
			anim.SetTrigger("jump");
		}
		jump = false;

		if (xvel > 0) {
			facingRight = true;
		} else if (xvel < 0) {
			facingRight = false;
		}
		// if statement is true, in air. if false, not
		rb2d.velocity = new Vector2(xvel * 1f, y_vel);

		anim.SetFloat ("speed", Mathf.Abs (xvel));
	}

	public void colliderEnabled(bool value) {
		if (colliders != null) {
			foreach (Collider2D collider in colliders) {
				if (collider != null) {
					collider.enabled = value;
					collider.isTrigger = !value;
				}
			}
		}
	}

	public void reset() {
		if (colliders == null||rb2d==null) {
			Start ();
		}
		transform.localPosition = Vector3.zero;
		colliderEnabled (true);
		rb2d.gravityScale = 1f;
		rb2d.velocity = Vector2.zero;
		xvel = yvel = 0f;
		_up = _down = _left = _right = 0;
		_godown = _goright = false;
		anim.SetBool ("dead", false);
	}

	public void spriteEnabled(bool value) {
		SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer> ();
		foreach (SpriteRenderer sprite in sprites) {
			sprite.enabled = value;
		}
	}


	public void disable() {
		reset ();
		rb2d.gravityScale = 0f;
		colliderEnabled (false);
		spriteEnabled (false);
	}

	void OnTriggerEnter2D(Collider2D collider) {
		master.triggerCheck (collider);
	}
}
