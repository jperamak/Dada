using UnityEngine;
using System.Collections;

public class HeroHealth : Damageable {

	public float ShieldAfterRespawn = 3.5f;
	public float ShieldAfterDamage = 1.5f;
	public float ShieldDelay = 0.2f;
	
	public void Kill(GameObject killer = null){
		base.TakeDamage(MaxHitpoints, killer);
	}

	//TODO: Here is where the shield should be applied
	public override void TakeDamage (float hitpoints, GameObject dealer){
		base.TakeDamage (hitpoints, dealer);

	}

}
