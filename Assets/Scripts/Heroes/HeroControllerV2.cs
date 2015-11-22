using UnityEngine;
using Dada.InputSystem;
using System.Collections;

[RequireComponent(typeof(Hero))]
public class HeroControllerV2 : HeroController { 

	protected Hero _hero;
	protected Rigidbody2D _rigidbody;
	private BoxCollider2D _boxCollider;
	
	//class private attributes
	protected Transform _groundCheck;			// A position marking where to check if the player is grounded.
	protected Transform _groundCheckLeft;		// A position marking where to check if the player is grounded.
	protected Transform _groundCheckRight;	// A position marking where to check if the player is grounded.
	protected Transform _groundCheckUpperLeft;		// A position markings to corner of area where to check if the player is grounded.
	protected Transform _groundCheckLowerRight;	// A position marking where to check if the player is grounded.
	protected Transform _slopeCheck;
	protected Transform _wallCheck;			// A position marking where to check if the player is grounded.
	protected Transform _crossairPivot;		// The point where the crossair is attached to
	protected Transform _crossair; 			// Crossair's transform, useful for calculating the shoot direction
	protected Transform _rangeWeaponHand;		// The hand that holds the ranged weapon
	protected Transform _weaponSpawner;		// the default spawn point of the weapon

    protected ParticleSystem _jumpCloud;

    private int _tauntIndex;				// The index of the taunts array indicating the most recent taunt.
	private bool _grounded = false;			// Whether or not the player is grounded.
	private bool _jump = false;
	private float _jumpStartTime;

	private float _jumpPressedLastTime = 0f;
	
	// Setting up initial references.
	protected virtual void Awake(){
		
		_hero 		= GetComponent<Hero>();
		_rigidbody 	= GetComponent<Rigidbody2D>();
		//_boxCollider = GetComponent<BoxCollider2D>();

		//if (_boxCollider == null)
		//	Debug.LogError("HeroController needs the Hero to have a BoxCollider2D");
        _jumpCloud = transform.Find("JumpCloud").GetComponent < ParticleSystem>();
		_rangeWeaponHand = transform.Find("Hand1");
		_groundCheck     = transform.Find("GroundCheck");
		_groundCheckLeft = transform.Find("GroundCheckLeft");
		_groundCheckRight = transform.Find("GroundCheckRight");
		_groundCheckUpperLeft = transform.Find("GroundCheckUpperLeft");
		_groundCheckLowerRight = transform.Find("GroundCheckLowerRight");
		_slopeCheck = transform.Find("SlopeCheck");

		_wallCheck 		 = transform.Find("WallCheck");
		_crossairPivot 	 = transform.Find("CrossairPivot");
		_crossair 		 = _crossairPivot.Find("Crossair");

	}


	protected virtual void FixedUpdate (){
		
		//no controller, no party
		if(_hero.PlayerInstance == null || _hero.PlayerInstance.Controller == null)
			return;
		
		ProcessMovement();
	}
	
	protected virtual void Update () {
		
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
	
	protected virtual void ProcessAim(){
		
		//rotate crossair and ranged weapon accordingly to current aim and hero's rotation

		// Option 1: The old way 
        float yAxis = _hero.PlayerInstance.Controller.YAxis;// *0.5f;
		float aimAngle = Mathf.Rad2Deg * Mathf.Asin(yAxis) + transform.rotation.eulerAngles.z;
		Vector3 newRotation = new Vector3(0, 0, aimAngle);
		Vector3 crossairRotation = newRotation;

        //correct crossair rotation due to negative scale of the x axis
        if (!IsFacingRight){
			aimAngle = Mathf.Rad2Deg * Mathf.Asin(yAxis) - transform.rotation.eulerAngles.z;
			crossairRotation = new Vector3(0, 0, 180 - aimAngle);
		}

		_crossairPivot.eulerAngles 	 = newRotation;
		_rangeWeaponHand.eulerAngles = newRotation;
		_crossair.eulerAngles 		 = crossairRotation;

		//correct ranged weapon spawnpoint due to scale change
		if(_hero.RangedWeapon != null)
			_hero.RangedWeapon.SpawnPoint.eulerAngles = crossairRotation;

        //correct melee weapon spawnpoint due to scale change
        if (_hero.MeleeWeapon != null)
            _hero.MeleeWeapon.SpawnPoint.eulerAngles = crossairRotation;
    }

	protected virtual void ProcessWeapons(){
		AbstractController _controller =_hero.PlayerInstance.Controller;

		//Use the ranged weapon from the muzzle
		if(_controller.GetButtonDown(VirtualKey.SHOOT))
			_hero.RangedWeapon.OnTriggerDown();
		else if(_controller.GetButtonUp(VirtualKey.SHOOT))
			_hero.RangedWeapon.OnTriggerUp();
		
		//use the melee weapon
		if(_controller.GetButtonDown(VirtualKey.MELEE))
			_hero.MeleeWeapon.OnTriggerDown();
		else if(_controller.GetButtonUp(VirtualKey.MELEE))
			_hero.MeleeWeapon.OnTriggerUp();
	}

	protected virtual  void ProcessSlopes(){
		float h = _hero.PlayerInstance.Controller.XAxis;
		h = Mathf.Abs(h) < 0.25f ? 0 : h;

		// If the player's horizontal velocity is greater than the _hero.MaxSpeed...
		if(Mathf.Abs(_rigidbody.velocity.x) > _hero.MaxSpeed * Mathf.Abs(h))
			// ... set the player's velocity to the _hero.MaxSpeed in the x axis.
			_rigidbody.velocity = new Vector2(Mathf.Sign(_rigidbody.velocity.x) * _hero.MaxSpeed * Mathf.Abs(h), _rigidbody.velocity.y);
		if (_grounded && !_jump && Physics2D.Linecast(
			transform.position + _slopeCheck.localPosition, transform.position + _slopeCheck.localPosition - _wallCheck.localPosition, LayerMask.GetMask("Ground")))
		{
			// Debug.Log("slope left");
			_rigidbody.AddForce(transform.up * -h * _hero.SlopeForce);
		}
		if (_grounded && !_jump && Physics2D.Linecast(
			transform.position + _slopeCheck.localPosition, transform.position + _slopeCheck.localPosition + _wallCheck.localPosition, LayerMask.GetMask("Ground")))
		{
			// Debug.Log("slope right");
			_rigidbody.AddForce(transform.up * h * _hero.SlopeForce);
		}
	}

	protected virtual void ProcessFlip(){
		float h = _hero.PlayerInstance.Controller.XAxis;
		h = Mathf.Abs(h) < 0.25f ? 0 : h;

		// If the input is moving the player right and the player is facing left...
		if(h > 0 && !IsFacingRight)
			// ... flip the player.
			Flip();
		// Otherwise if the input is moving the player left and the player is facing right...
		else if(h < 0 && IsFacingRight)
			// ... flip the player.
			Flip();
	}

	protected virtual void ProcessMovement (){

		AbstractController _controller = _hero.PlayerInstance.Controller;
		
		// Cache the horizontal input.
		float h = _hero.PlayerInstance.Controller.XAxis;
		
		//h = Mathf.Abs(h) < 0.25f ? 0 : h;
		h = JoystickDeadzone(h, 0.25f);
		
		// The Speed animator parameter is set to the absolute value of the horizontal input.
		//_anim.SetFloat("Speed", _grounded ? Mathf.Abs(h) : 0);
		
		// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached _hero.MaxSpeed yet...
		if(h * _rigidbody.velocity.x < _hero.MaxSpeed)
			// ... add a force to the player.
			_rigidbody.AddForce(transform.right * h * _hero.MoveForce, ForceMode2D.Force);

		
		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
		_grounded = IsGrounded();


		if (_controller.GetButtonDown(VirtualKey.JUMP))
			_jumpPressedLastTime = Time.time;

		// Jump if the jump button has been pressed in last 0.1s and the character is on solid ground
		if (Time.time - _jumpPressedLastTime < 0.1f && _grounded)
		{
			
			//_jumpStartTime = Time.time;
			//_jumpStart = true;
			//_jump = true;
			if (_hero.JumpSound != null)
				_hero.JumpSound.PlayEffect();
			_rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _hero.JumpSpeed);
			
		}

		//if (_grounded)
		//	_anim.SetBool("Slide", false);
		
		// If the player should jump...
		//if(_jumpStart)
		//{
			// Set the Jump animator trigger parameter.
			//_anim.SetBool("Slide", false);
			//_anim.SetTrigger("Jump");
			
			// Play a random jump audio clip.
			//if (JumpClips.Length > 0)
			//	DadaAudio.PlayRandom(JumpClips);
		//	if (_hero.JumpSound != null)
		//		_hero.JumpSound.PlayEffect();
			
			// Add a vertical force to the player.
		//	_rigidbody.AddForce(transform.up * _hero.JumpForce);
		//	_rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _hero.JumpSpeed);
			
			// Make sure the player can't jump again until the jump conditions from Update are satisfied.
		//	_jumpStart = false;
		//}
	//	if (_jump)
	//	{
	//		_rigidbody.AddForce( transform.up * _hero.JumpForce * _hero.JumpAirModifier);
	//	}
		/*
		if (_walljump > 0 && !_grounded)
		{
			//_anim.SetBool("Slide", false);
			//_anim.SetTrigger("Jump");
			
			// Play a random jump audio clip.
			//if (JumpClips.Length > 0)
			//	DadaAudio.PlayRandom(JumpClips);
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
		}*/
	}
	
	protected virtual void ProcessJump(){

		AbstractController _controller = _hero.PlayerInstance.Controller;
		/*
		if (_controller.GetButtonDown(VirtualKey.JUMP) && Physics2D.Linecast(transform.position, transform.position  - _wallCheck.localPosition, 1 << LayerMask.NameToLayer("Ground")))
		{
			_jumpStartTime = Time.time;
			_jump = true; 
			_walljump = 1;
			//_anim.SetBool("Slide", true);
		}
		
		else if (_controller.GetButtonDown(VirtualKey.JUMP) && Physics2D.Linecast(transform.position, transform.position + _wallCheck.localPosition, 1 << LayerMask.NameToLayer("Ground")))
		{
			_jumpStartTime = Time.time;
			_jump = true;
			
			_walljump = 2;
			//_anim.SetBool("Slide", true);
		}
		else
		{
			_walljump = 0;
		}*/
        if (_controller.GetButton(VirtualKey.JUMP))
        {
            _rigidbody.gravityScale = 0.9f;
            _jumpCloud.Play();
        }
        else
        {
            _rigidbody.gravityScale = 3.0f;
            _jumpCloud.Stop();
        }
		// If the jump button is pressed and the player is grounded then the player should jump.
		//Debug.Log (_rigidbody.velocity.y);
	

		//else if (_controller.GetButtonUp(VirtualKey.JUMP) || Time.time - _jumpStartTime > _hero.JumpLength )
		//	_jump = false;
	}

	protected virtual void Flip (){
		
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	private bool IsGrounded(){
		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
//		return Physics2D.Linecast(transform.position, _groundCheck.position, _hero.JumpOn) 
//			||  Physics2D.Linecast(transform.position, _groundCheckLeft.position, _hero.JumpOn) 
//				|| Physics2D.Linecast(transform.position, _groundCheckRight.position, _hero.JumpOn); //LayerMask.GetMask(new string[] { "Ground", "Rubble",  }));
	
		return (Physics2D.OverlapArea( _groundCheckUpperLeft.position, _groundCheckLowerRight.position, _hero.JumpOn) != null);
	}  




	// Adjust joystick input (-1..1) so that input which has absolute value of minimum or smaller will return 0f
	// and larger values will be scaled so they are in the range -1..1
	private float JoystickDeadzone(float input, float minimum){

		float result = Mathf.Abs(input);
		result -= minimum;
		if (result < 0f)
			return 0f;

		result = result * (1f/(1f-minimum));

		if (input < 0.0f)
			return -result;

		return result;
	}
}
