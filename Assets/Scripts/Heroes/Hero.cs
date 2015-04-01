using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour { 

	//Utility attributes
	public Player PlayerInstance{get; private set;}
	public Weapon MeleeWeapon{get; private set;} 	// Reference to the assigned melee weapon
	public Weapon RangedWeapon{get; private set;}	// Reference to the assigned range weapon

	//Public attributes
	public float MoveForce = 365.0f;		// Amount of force added to move the player left and right.
    public float SlopeForce = 365.0f;	
    public float MaxSpeed = 3f;				// The fastest the player can travel in the x axis.
	public float JumpForce = 1000f;			// Amount of force added when the player jumps.
    public float JumpAirModifier = 0.02f;
    public float JumpLength = 0.5f;         // Length of the jump
	public LayerMask JumpOn;				// Layermask that specify the elements the player can jump on
	public LayerMask JumpOnWalls;			// Layermask that specify the walls the player can jump on
	public LayerMask WalkOnSlopes;			// Layermask that specify the walls the player can jump on
    public SoundEffect JumpSound;
	public int TeamNumber;
	public Sprite[] TeamColors;

	private Transform _rangeWeaponHand;		// The hand that holds the ranged weapon
	private Transform _meleeWeaponHand;		// The hand that holds the melee weapon
	private Transform _body;

	void Awake(){
		_rangeWeaponHand = transform.Find("Hand1");
		_meleeWeaponHand = transform.Find("Hand2");
		_body = transform.Find("Body/body1");
		JumpSound = DadaAudio.GetSoundEffect(JumpSound);
	}

	void Start(){
		if(TeamNumber < TeamColors.Length)
			_body.GetComponent<SpriteRenderer>().sprite = TeamColors[TeamNumber];
	}


	// The player soul is incarnated in the hero's body. Get its controller and find weapon references
	public void SetPlayer(Player player){
		PlayerInstance = player;
		MeleeWeapon = _meleeWeaponHand.GetComponentInChildren<Weapon>();
		RangedWeapon = _rangeWeaponHand.GetComponentInChildren<Weapon>();

		TeamNumber = player.TeamNumber;
		name = "Player "+player.Number;

		if(TeamNumber < TeamColors.Length)
			_body.GetComponent<SpriteRenderer>().sprite = TeamColors[TeamNumber];

	}

	public void GiveWeapon(Weapon weapon)
    {
        Destroy(RangedWeapon.gameObject);
		RangedWeapon = weapon;
		RangedWeapon.transform.position = _rangeWeaponHand.position;
		RangedWeapon.transform.rotation = _rangeWeaponHand.rotation;
        if (transform.localScale.x == -1)
			RangedWeapon.transform.localScale = new Vector3(1, 1, -1);
    }
}
