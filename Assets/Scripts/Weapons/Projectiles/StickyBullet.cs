using UnityEngine;
using System.Collections;

public class StickyBullet : Projectile {

	protected bool _isStick = false;
	private int bouncesCount = 0;
	void OnCollisionEnter2D(Collision2D coll){

		//stick only at the second contact
		if(bouncesCount == 0){
			TriggerEffects();
			bouncesCount++;
		}
		else if(!_isStick){
			Destroy(GetComponent<Rigidbody2D>());
			Destroy(GetComponent<Collider2D>());
			transform.parent = coll.transform;
			_isStick = true;

		}
	}


}
