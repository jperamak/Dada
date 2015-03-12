using UnityEngine;
using System.Collections;

public class MachineGun : RangedWeapon {
	
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


}
