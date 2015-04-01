using UnityEngine;
using Dada.InputSystem;
using System.Collections;

public class HeroJetpackController : HeroController {

	private bool _isFlying = false;

	protected override void Update (){

		//no controller, no party
		if(_hero.PlayerInstance == null || _hero.PlayerInstance.Controller == null)
			return;

		//Flip
		ProcessFlip();
		
		//Aim
		ProcessAim();
		
		//Weapons
		ProcessWeapons();
	}

	//move using jetpack
	protected override void ProcessMovement (){
		bool isJumpPressed 	= _hero.PlayerInstance.Controller.GetButton(VirtualKey.JUMP);

		if(isJumpPressed && _rigidbody.velocity.magnitude < _hero.MaxSpeed)
			_rigidbody.AddForce(_crossair.right * _hero.MoveForce);

	}
}
