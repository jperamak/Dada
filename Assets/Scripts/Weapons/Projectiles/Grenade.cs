using UnityEngine;
using System.Collections;

public class Grenade : Projectile {

	public bool DetonateOnContact = true;
	public bool ExplodeOnContact = false;
    public bool ExplodeOnPlayerContact = false;
    public float DetonationDelay = 1.5f;
	public bool blinkRed = false;
    public bool randomRotate = false;

	private SpriteRenderer _renderer;
	private Rigidbody2D _rigidbody;
	private float _contactTime = 0;
	private bool exploded = false;


	void Start(){
		_renderer = transform.GetComponentInChildren<SpriteRenderer>();
		_rigidbody = GetComponent<Rigidbody2D>();
        if ( randomRotate)
            _rigidbody.angularVelocity = Random.Range(-180f,180f);
		//start immediately the countdown DetonateOnContact is false
		if(!DetonateOnContact){
			_contactTime = Time.time;
			StartCoroutine(TickTack());
		}
	}

	void OnCollisionEnter2D(Collision2D coll){

        if (ExplodeOnContact)
            Explode();
        else if (ExplodeOnPlayerContact)
            if (coll.collider.gameObject.tag == "Player")
                Explode();

        //start the countdown at the first contact.
        //no need for collisions when DetonateOnContact is false
        else if (DetonateOnContact && _contactTime == 0)
        {
            _contactTime = Time.time;
            StartCoroutine(TickTack());
        }
	}

	//turn the gameobject so it follows the gravity
	protected virtual void FixedUpdate (){

		Vector2 velocity = _rigidbody.velocity;
        if (!randomRotate)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x);
            transform.eulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);
        }
	}

	private void Explode(){
		TriggerEffects();
		exploded = true;
	}

	private IEnumerator TickTack(){
		while(!exploded){
			if (blinkRed) {
				if(_renderer.color == Color.white)
					_renderer.color = Color.red;
				else
					_renderer.color = Color.white;
			}
			if(Time.time >= _contactTime + DetonationDelay)
				Explode();
			yield return new WaitForSeconds(0.25f);
		}
	}



}
