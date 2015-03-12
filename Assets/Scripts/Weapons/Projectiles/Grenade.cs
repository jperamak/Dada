using UnityEngine;
using System.Collections;

public class Grenade : Projectile {

	public bool DetonateOnContact = true;
	public float DetonationDelay = 1.5f;
	
	private SpriteRenderer _renderer;
	private float _contactTime = 0;
	private bool exploded = false;

	void Start(){
		_renderer = transform.GetComponentInChildren<SpriteRenderer>();

		//start immediately the countdown DetonateOnContact is false
		if(!DetonateOnContact){
			_contactTime = Time.time;
			StartCoroutine(TickTack());
		}
	}

	void OnCollisionEnter2D(Collision2D coll){

		//start the countdown at the first contact.
		//no need for collisions when DetonateOnContact is false
		if(DetonateOnContact && _contactTime == 0){
			Debug.Log(name+" collided with "+coll.gameObject.name);
			_contactTime = Time.time;
			StartCoroutine(TickTack());
		}
	}

	private void Explode(){
		Debug.Log("Granade of "+Owner.name+" exploded");
		TriggerEffects();
		exploded = true;
	}

	private IEnumerator TickTack(){
		while(!exploded){
			if(_renderer.color == Color.white)
				_renderer.color = Color.red;
			else
				_renderer.color = Color.white;

			if(Time.time >= _contactTime + DetonationDelay)
				Explode();
			yield return new WaitForSeconds(0.25f);
		}
	}



}
