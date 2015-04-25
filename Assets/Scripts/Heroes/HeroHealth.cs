using UnityEngine;
using System.Collections;

public class HeroHealth : Damageable {

	public bool IsShielded{get; private set;}
	public float ShieldAfterRespawn = 3.5f;
	public float ShieldAfterDamage = 1.5f;
	public float ShieldDelay = 0.2f;
	public bool FriendlyFireEnabled = false;

	private bool _shieldIsSetting = false;
	private Team _heroTeam;

	void Start(){
		_heroTeam = GetComponent<Hero>().PlayerInstance.InTeam;
		IsShielded = true;
		Invoke("RemoveShield",ShieldAfterRespawn);
	}

	public void Kill(GameObject killer = null){
		base.TakeDamage(MaxHitpoints, killer);
	}

	//TODO: Here is where the shield should be applied
	public override void TakeDamage (float hitpoints, GameObject dealer){

		if(!IsShielded){

			//if friendly fire is disabled and dealer is in the same team, don't apply any damage
			if(dealer != null){
				Hero dealerHero = dealer.GetComponent<Hero>();
				if( !FriendlyFireEnabled && dealerHero != null && dealerHero.PlayerInstance.InTeam == _heroTeam)
					return;

			}


			base.TakeDamage (hitpoints, dealer);

			if(!_shieldIsSetting){
				_shieldIsSetting = true;
				Invoke("SetShield",ShieldDelay);
				Invoke("RemoveShield",ShieldDelay + ShieldAfterDamage);
			}
		}
	}

	private void SetShield(){
		IsShielded = true;
		_shieldIsSetting = false;
	}

	private void RemoveShield(){
		IsShielded = false;
	}

}
