using UnityEngine;
using System.Collections;

public class HeroHealth : Damageable {

	public bool IsShielded{get; private set;}
	public float ShieldAfterRespawn = 3.5f;
	public float ShieldAfterDamage = 1.5f;
	public float ShieldDelay = 0.2f;

	private bool _shieldIsSetting = false;

	void Start(){
		IsShielded = true;
		Invoke("RemoveShield",ShieldAfterRespawn);
	}

	public void Kill(GameObject killer = null){
		base.TakeDamage(MaxHitpoints, killer);
	}

	//TODO: Here is where the shield should be applied
	public override void TakeDamage (float hitpoints, GameObject dealer){


		if(!IsShielded){
			Debug.Log("take "+hitpoints+" damage. --> "+_currentHitpoints);
			base.TakeDamage (hitpoints, dealer);

			if(!_shieldIsSetting){
				_shieldIsSetting = true;
				Invoke("SetShield",ShieldDelay);
				Invoke("RemoveShield",ShieldDelay + ShieldAfterDamage);
			}
		}
	}

	private void SetShield(){
		Debug.Log("shielded");
		IsShielded = true;
		_shieldIsSetting = false;
	}

	private void RemoveShield(){
		Debug.Log("No shielded");
		IsShielded = false;
	}

}
