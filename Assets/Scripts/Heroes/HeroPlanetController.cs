﻿using UnityEngine;
using Dada.InputSystem;
using System.Collections;

[RequireComponent(typeof(Hero))]
public class HeroPlanetController : HeroController {
	
	private bool _grounded = false;			// Whether or not the player is grounded.
	private int _walljump = 0;              // whether or not the player can walljump
	private bool _jumpStart = false;
	private bool _jump = false;
	private float _jumpStartTime;

	protected override void FixedUpdate (){
		
		//no controller, no party
		if(_hero.PlayerInstance == null || _hero.PlayerInstance.Controller == null)
			return;
		
		ProcessMovement();
	}
	
	protected override void Update () {
		
		//no controller, no party
		if(_hero.PlayerInstance == null || _hero.PlayerInstance.Controller == null)
			return;
		
		//Jump
		ProcessJump();
		
		//Slopes
		ProcessSlopes();
		
		//Flip
		ProcessFlip();
		
		//Aim
		ProcessAim();
		
		//Weapons
		ProcessWeapons();
	}

	
	protected override void ProcessWeapons(){
		AbstractController c = _hero.PlayerInstance.Controller;
		//Use the ranged weapon from the muzzle
		if(c.GetButtonDown(VirtualKey.SHOOT))
			_hero.RangedWeapon.OnTriggerDown(_crossair);
		else if(c.GetButtonUp(VirtualKey.SHOOT))
			_hero.RangedWeapon.OnTriggerUp();
		
		//use the melee weapon
		if(c.GetButtonDown(VirtualKey.MELEE))
			_hero.MeleeWeapon.OnTriggerDown(_crossair);
		else if(c.GetButtonUp(VirtualKey.MELEE))
			_hero.MeleeWeapon.OnTriggerUp();
	}
	
	
	//NOTE: All calculations involving velocity and directions should be done in the hero's local space
	//because we cannot assume that the hero is aligned with the world x axis
	protected override void ProcessMovement(){
		
		float h = _hero.PlayerInstance.Controller.XAxis;
		
		//Time for a new joystick? :P
		h = Mathf.Abs(h) < 0.25f ? 0 : h;
		
		//calculate velocity in local space
		Vector2 localVelocity = transform.InverseTransformVector(_rigidbody.velocity);
		
		//player is moving
		if(h != 0){
			
			//Apply movement only if the total velocity does not exceed the maximum speed (both to the left and to the right)
			if(h > 0){
				if(h * localVelocity.x < _hero.MaxSpeed)
					_rigidbody.AddForce(transform.right * h * _hero.MoveForce);
			}
			else{
				if(h * localVelocity.x > -_hero.MaxSpeed)
					_rigidbody.AddForce(transform.right * h * _hero.MoveForce);
			}
		}
		
		//FIXME: it does not behave well with attractor points, it cancels part of the horizontal attraction 
		// If the player's horizontal velocity is greater than the maxSpeed
		// set the player's velocity to the maxSpeed in the x axis.
		if(Mathf.Abs(localVelocity.x) > _hero.MaxSpeed * Mathf.Abs(h)){
			localVelocity.x = Mathf.Sign(localVelocity.x) * _hero.MaxSpeed * Mathf.Abs(h);
			_rigidbody.velocity = transform.TransformVector(localVelocity);
		}
		
		// If the player should jump...
		if(_jumpStart){
			
			// Play a random jump audio clip.
			if (_hero.JumpSound != null)
				_hero.JumpSound.PlayEffect();
			
			// Add a vertical force to the player.
			_rigidbody.AddForce(transform.up * _hero.JumpForce);
			
			// Make sure the player can't jump again until the jump conditions from Update are satisfied.
			_jumpStart = false;
		}
		if (_jump){
			_rigidbody.AddForce( transform.up * _hero.JumpForce * _hero.JumpAirModifier);
		}
		if (_walljump > 0 && !_grounded){
			
			// Play a random jump audio clip.
			if (_hero.JumpSound != null)
				_hero.JumpSound.PlayEffect();
			
			// Add a vertical force to the player.
			if (_walljump == 1)
			{
				_rigidbody.velocity = Vector2.zero;
				_rigidbody.AddForce(new Vector2(_hero.JumpForce, _hero.JumpForce));
			}
			else
			{
				_rigidbody.velocity = Vector2.zero;
				_rigidbody.AddForce(new Vector2(-_hero.JumpForce, _hero.JumpForce));
			}
			_walljump = 0;
		}
	}
	
	protected override  void ProcessSlopes(){
		
		float h = _hero.PlayerInstance.Controller.XAxis;
		
		if (_grounded && !_jump && Physics2D.Linecast(_slopeCheck.position,_slopeCheck.position - _wallCheck.localPosition, _hero.WalkOnSlopes))
		{
			// Debug.Log("slope left");
			_rigidbody.AddForce(transform.up * -h * _hero.SlopeForce);
		}
		if (_grounded && !_jump && Physics2D.Linecast(
			_slopeCheck.position, _slopeCheck.position + _wallCheck.localPosition, _hero.WalkOnSlopes))
		{
			// Debug.Log("slope right");
			_rigidbody.AddForce(transform.up * h * _hero.SlopeForce);
		}
	}
	
	
	
	protected override void ProcessJump(){
		bool isBtnJumpDown 	= _hero.PlayerInstance.Controller.GetButtonDown(VirtualKey.JUMP);
		
		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
		_grounded = Physics2D.OverlapPoint(_groundCheck.position, _hero.JumpOn) || 
			Physics2D.Linecast(transform.position, _groundCheckLeft.position, _hero.JumpOn) || 
				Physics2D.Linecast(transform.position, _groundCheckRight.position, _hero.JumpOn);
		
		if (isBtnJumpDown && Physics2D.Linecast(transform.position, transform.position  - _wallCheck.localPosition, _hero.JumpOnWalls.value)){
			_jumpStartTime = Time.time;
			_jump = true; 
			_walljump = 1;
		}
		
		else if (isBtnJumpDown && Physics2D.Linecast(transform.position, transform.position + _wallCheck.localPosition, _hero.JumpOnWalls.value)){
			_jumpStartTime = Time.time;
			_jump = true;
			_walljump = 2;
		}
		else
			_walljump = 0;
		
		// If the jump button is pressed and the player is grounded then the player should jump.
		if (isBtnJumpDown && _grounded){
			_jumpStartTime = Time.time;
			_jumpStart = true;
			_jump = true;
		}
		else if (isBtnJumpDown || Time.time - _jumpStartTime > _hero.JumpLength )
			_jump = false;
	}
	
	protected override void ProcessFlip(){
		float h = _hero.PlayerInstance.Controller.XAxis;
		
		// If the input is moving the player right and the player is facing left flip the player.
		if(h > 0 && !_facingRight)
			Flip();
		
		// Otherwise if the input is moving the player left and the player is facing right flip the player.
		else if(h < 0 && _facingRight)
			Flip();
	}
	
	protected override void Flip (){
		// Switch the way the player is labelled as facing.
		_facingRight = !_facingRight;
		
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
