﻿using UnityEngine;
using System.Collections;

public class BouncingBullet : Projectile {

	public int MaxBounces = 1;
	private int _bounces = 0;
	protected GameObject _targetHit;

	void Start() {
		transform.eulerAngles = new Vector3( 0f,0f,Random.Range(0f,360f) );
	}

	void OnCollisionEnter2D(Collision2D coll){

		if(_bounces < MaxBounces && coll != null){
			_targetHit = coll.gameObject;
			TriggerEffects();
			_bounces++;

			if(_bounces > MaxBounces)
				Destroy(gameObject);
		}
	}


	public override void TriggerEffects(){
		
		if(_effects != null){
			for(int i=0;i<_effects.Length;i++){
				_effects[i].Owner = Owner;
				_effects[i].Trigger(_targetHit);
			}
		}
	}
}
