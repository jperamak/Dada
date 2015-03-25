using UnityEngine;
using Dada.InputSystem;
using System.Collections;

public class Hero : MonoBehaviour { 

	//Utility attributes
	public Player PlayerInstance{get; private set;}

	//Public attributes
	public float MoveForce = 365.0f;		// Amount of force added to move the player left and right.
	public float MaxSpeed = 3f;				// The fastest the player can travel in the x axis.
	public float JumpForce = 1000f;			// Amount of force added when the player jumps.
    public float JumpAirModifier = 0.02f;
    public float JumpLength = 0.5f;         // Length of the jump
    public LayerMask JumpOn;				// Layermask that specify the elements the player can jump on
	public AudioClip[] JumpClips;			// Array of clips for when the player jumps.

	//class private attributes
	private AbstractController _controller;	// Controller used to query the player's input
	private Transform _groundCheck;			// A position marking where to check if the player is grounded.
    private Transform _groundCheckLeft;			// A position marking where to check if the player is grounded.
    private Transform _groundCheckRight;			// A position marking where to check if the player is grounded.
    private Transform _wallCheck;			// A position marking where to check if the player is grounded.
    private Transform _crossairPivot;		// The point where the crossair is attached to
	private Transform _crossair; 			// Crossair's transform, useful for calculating the shoot direction
	private Transform _rangeWeaponHand;		// The hand that holds the ranged weapon

	private Animator _anim;					// Reference to the player's animator component.
	private Weapon _melee;					// Reference to the assigned melee weapon
	private Weapon _ranged;					// Reference to the assigned ranged weapon

	private int _tauntIndex;				// The index of the taunts array indicating the most recent taunt.
	private bool _grounded = false;			// Whether or not the player is grounded.
	private int _walljump = 0;              // whether or not the player can walljump
	private bool _facingRight = true;		// For determining which way the player is currently facing.
	private bool _jumpStart = false;
    private bool _jump = false;
    private float _jumpStartTime;

	// Setting up initial references.
	void Awake(){
		_rangeWeaponHand = transform.Find("Hand1");
        _groundCheck     = transform.Find("GroundCheck");
        _groundCheckLeft = transform.Find("GroundCheckLeft");
        _groundCheckRight = transform.Find("GroundCheckRight");
        _wallCheck       = transform.Find("WallCheck");
		_crossairPivot 	 = transform.Find("CrossairPivot");
		_crossair 		 = _crossairPivot.Find("Crossair");
		_anim = GetComponent<Animator>();

	}

	// The player soul is incarnated in the hero's body. Get its controller and find weapon references
	public void SetPlayer(Player player){
		PlayerInstance = player;
		_controller = player.Controller;
		
		_melee = GetComponentInChildren<MeleeWeapon>();
		_ranged = GetComponentInChildren<RangedWeapon>();
		name = "Player "+player.Number;
	}

	public void GiveWeapon(Weapon weapon)
    {
        Destroy(_ranged.gameObject);
        _ranged = weapon;
        _ranged.transform.position = _rangeWeaponHand.position;
        _ranged.transform.rotation = _rangeWeaponHand.rotation;
        if (!_facingRight)
            _ranged.transform.localScale = new Vector3(1, 1, -1);
    }

	void Update () {

		//no controller, no party
		if(_controller == null)
			return;

		//Jump
		ProcessJump();

		//Aim
		float yAxis = _controller.YAxis;
		float aimAngle = Mathf.Rad2Deg * Mathf.Asin(yAxis);
		Vector3 newRotation = new Vector3(0, 0, aimAngle);

		//rotate crossair and ranged weapon accordingly to current aim
		_crossairPivot.eulerAngles 	 = newRotation;
		_rangeWeaponHand.eulerAngles = newRotation;

		//correct crossair rotation due to negative scale of the x axis
		if(!_facingRight)
			newRotation.z = 180 - newRotation.z;

		//we can now correctly pass the crossair transform to the weapon to spawn projectiles
		_crossair.eulerAngles = newRotation;


		//Use the ranged weapon from the muzzle
		if(_controller.GetButtonDown(VirtualKey.SHOOT))
			_ranged.OnTriggerDown(_crossair);
		else if(_controller.GetButtonUp(VirtualKey.SHOOT))
			_ranged.OnTriggerUp();
		
		//use the melee weapon
		if(_controller.GetButtonDown(VirtualKey.MELEE))
			_melee.OnTriggerDown(_crossair);
		else if(_controller.GetButtonUp(VirtualKey.MELEE))
			_melee.OnTriggerUp();


	}

	void FixedUpdate ()
	{
		//no controller, no party
		if(_controller == null)
			return;

		// Cache the horizontal input.
		float h = _controller.XAxis;
		
		// The Speed animator parameter is set to the absolute value of the horizontal input.
		//_anim.SetFloat("Speed", _grounded ? Mathf.Abs(h) : 0);
		
		// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
		if(h * GetComponent<Rigidbody2D>().velocity.x < MaxSpeed)
			// ... add a force to the player.
			GetComponent<Rigidbody2D>().AddForce(Vector2.right * h * MoveForce);
		
		// If the player's horizontal velocity is greater than the maxSpeed...
		if(Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) > MaxSpeed * Mathf.Abs(h))
			// ... set the player's velocity to the maxSpeed in the x axis.
			GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sign(GetComponent<Rigidbody2D>().velocity.x) * MaxSpeed * Mathf.Abs(h), GetComponent<Rigidbody2D>().velocity.y);
		
		// If the input is moving the player right and the player is facing left...
		if(h > 0 && !_facingRight)
			// ... flip the player.
			Flip();
		// Otherwise if the input is moving the player left and the player is facing right...
		else if(h < 0 && _facingRight)
			// ... flip the player.
			Flip();

		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
        _grounded = Physics2D.Linecast(transform.position, _groundCheck.position, JumpOn) || Physics2D.Linecast(transform.position, _groundCheckLeft.position, JumpOn) || Physics2D.Linecast(transform.position, _groundCheckRight.position, JumpOn);//LayerMask.GetMask(new string[] { "Ground", "Rubble",  }));

		//if (_grounded)
		//	_anim.SetBool("Slide", false);
		
		// If the player should jump...
		if(_jumpStart)
		{
			// Set the Jump animator trigger parameter.
			//_anim.SetBool("Slide", false);
			//_anim.SetTrigger("Jump");
			
			// Play a random jump audio clip.
			if (JumpClips.Length > 0)
				DadaAudio.PlayRandom(JumpClips);

			
			// Add a vertical force to the player.
			GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, JumpForce));
			
			// Make sure the player can't jump again until the jump conditions from Update are satisfied.
			_jumpStart = false;
		}
        if (_jump)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, JumpForce * JumpAirModifier));
        }
		if (_walljump > 0 && !_grounded)
		{
			//_anim.SetBool("Slide", false);
			//_anim.SetTrigger("Jump");

			// Play a random jump audio clip.
			if (JumpClips.Length > 0)
				DadaAudio.PlayRandom(JumpClips);

			// Add a vertical force to the player.
            if (_walljump == 1)
			{
				GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                GetComponent<Rigidbody2D>().AddForce(new Vector2(JumpForce, JumpForce));
			}
			else
			{
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                GetComponent<Rigidbody2D>().AddForce(new Vector2(-JumpForce, JumpForce));
			}
            _walljump = 0;
		}
	}
	
	void ProcessJump(){

		var sliding = false;

        if (_controller.GetButtonDown(VirtualKey.JUMP) && Physics2D.Linecast(transform.position, transform.position  - _wallCheck.localPosition, 1 << LayerMask.NameToLayer("Ground")))
		{
            _jumpStartTime = Time.time;
            _jump = true; 
            _walljump = 1;
			//_anim.SetBool("Slide", true);
			sliding = true;
		}

        else if (_controller.GetButtonDown(VirtualKey.JUMP) && Physics2D.Linecast(transform.position, transform.position + _wallCheck.localPosition, 1 << LayerMask.NameToLayer("Ground")))
		{
            _jumpStartTime = Time.time;
            _jump = true;

			_walljump = 2;
			//_anim.SetBool("Slide", true);
			sliding = true;
		}
		else
		{
			_walljump = 0;
			//if (!sliding)
			//	_anim.SetBool("Slide", false);
		}
		// If the jump button is pressed and the player is grounded then the player should jump.
        if (_controller.GetButtonDown(VirtualKey.JUMP) && _grounded)
        {
            _jumpStartTime = Time.time;
            _jumpStart = true;
            _jump = true;
        }
        else if (_controller.GetButtonUp(VirtualKey.JUMP) || Time.time - _jumpStartTime > JumpLength )
            _jump = false;
	}

	void Flip (){
		// Switch the way the player is labelled as facing.
		_facingRight = !_facingRight;
		
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
