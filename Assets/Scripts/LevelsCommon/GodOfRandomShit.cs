using UnityEngine;
using System.Collections;

public class GodOfRandomShit : MonoBehaviour {

	public float TimeToTravel = 15.0f;
	public float WaitTimeAtStops = 30.0f;
	public float verticalMovement = 0.0f;
	public float TimeBeforePoop;
	public float DistancePoopOffset;
	public Transform StartPoint;
	public Transform FinalPoint;
	public Transform PoopAboutAtPoint;
	public GameObject[] WeaponPickupPrefabs;
    public SoundEffect PoopSound;

	private Transform _goingTo;
	private float _distance;
	private Vector2 _direction;
	private Vector2 _initialPos;
	private bool _going = false;
	private float _passedTime = 0;
	private float _totTime = 0; 

	private Animator _anim;
	private float altitude;
	private bool _pooping = false;
	private bool _pooped = false;
	private float _randOffset;


	// Use this for initialization
	void Start () {
        PoopSound = DadaAudio.GetSoundEffect(PoopSound);
		_anim = GetComponent<Animator>();
		altitude = transform.position.y;
		_goingTo = FinalPoint;


		Goto(FinalPoint.position,TimeToTravel);
	}
	
	// Update is called once per frame
	void Update () {

		Vector2 pos = transform.position;

		float distFromPoopPoint = (PoopAboutAtPoint.position.x - transform.position.x);

		if(Mathf.Abs(distFromPoopPoint + _randOffset) < 1.0f && !_pooping && !_pooped){
			_anim.SetTrigger("AboutToPoop");
			_pooping = true;
			Invoke("Plop",TimeBeforePoop);
		}

		if(!_pooping)
			pos.y = altitude + Mathf.Sin(Time.time * 2f ) * verticalMovement;

		bool isAnimIdle = _anim.GetCurrentAnimatorStateInfo(0).IsTag("Idle");
		if(!_pooping && isAnimIdle && _going){
			_passedTime += Time.deltaTime;
			float dist = Mathf.Lerp(0,_distance,_passedTime/_totTime);
			pos.x = (_initialPos + (_direction * dist)).x;

			if( _totTime - _passedTime <= 0){
				_going = false;
				Invoke("WaitAtPoint",WaitTimeAtStops);
			}
		}

		transform.position = pos;
	}

	private void Plop(){
		_anim.SetTrigger("Poop");
        if (PoopSound != null)
            PoopSound.PlayEffect();
		Instantiate(WeaponPickupPrefabs[Random.Range(0, WeaponPickupPrefabs.Length)], transform.position, Quaternion.identity);
		_pooping = false;
		_pooped = true;
	}

	private void WaitAtPoint(){

		if(_goingTo == StartPoint)
			_goingTo = FinalPoint;
		else
			_goingTo = StartPoint;

		Goto(_goingTo.position,TimeToTravel);
	}

	private void Goto(Vector2 target, float seconds){

		_randOffset = Random.Range(-DistancePoopOffset,DistancePoopOffset);
		_pooped = false;
		_passedTime = 0;
		_initialPos = transform.position;
		_direction = (target - _initialPos);
		_distance = _direction.magnitude;
		_direction.Normalize();
		_totTime = seconds;
		_going = true;
	}
}