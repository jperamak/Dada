using UnityEngine;
using System.Collections;

public class SimpleBullet : Projectile {

	protected GameObject _targetHit;

	void OnCollisionEnter2D(Collision2D coll){

		//A simple bullet can hit only one target
		if(_targetHit == null && coll.gameObject.GetComponent<SimpleBullet>() == null){
			_targetHit = coll.gameObject;
			TriggerEffects();
		}
	}

	protected override void TriggerEffects(){
		
		if(_effects != null){
			for(int i=0;i<_effects.Length;i++){
				_effects[i].Owner = Owner;
				_effects[i].OnEnd += OnEffectFinshed;
				_effects[i].Trigger(_targetHit);
			}
		}
	}
}
