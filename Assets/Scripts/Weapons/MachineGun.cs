using UnityEngine;
using System.Collections;

public class MachineGun : RangedWeapon {

	public bool ProjectileRandomRotation = false;

	private bool _triggerDown = false;

	public override void OnTriggerDown (){
		_triggerDown = true;
	}

	public override void OnTriggerUp (){
		_triggerDown = false;
	}

	void Update(){
		if(_triggerDown)
			base.OnTriggerDown();
	}

	protected override Projectile Shoot (){


		Projectile p =  base.Shoot ();
		if(ProjectileRandomRotation && p != null){
			p.transform.Rotate(new Vector3(0,0,1), Random.Range(0f,360f));
		}

		return p;
	}


}
