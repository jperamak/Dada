using UnityEngine;
using System.Collections;

public class Damage : Effect {

	public bool DamageOwner = true;
	public float DamageAmount;

	//Apply damage to all targets assigned
	protected override void Execute (){
		if(_targets != null){
			for(int i=0; i<_targets.Length; i++){


				//don't apply damage to owner if the option is checked
				if(!DamageOwner &&_targets[i] != Owner){
					Damageable dmg = _targets[i].GetComponent<Damageable>();
					if(dmg != null){
						dmg.TakeDamage(DamageAmount,Owner);
					}
				}
                else if (DamageOwner)
                {
                    Damageable dmg = _targets[i].GetComponent<Damageable>();
                    if (dmg != null)
                    {
                        dmg.TakeDamage(DamageAmount);
                    }
                }
			}
		}
	}
}
