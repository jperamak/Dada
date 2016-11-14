using UnityEngine;
using System.Collections;

public class MeleeWeapon : Weapon {

	public Projectile Strike;


	void Start(){
		if(SpawnPoint == null)
			SpawnPoint = transform.FindChild("Spawner");
	}



	protected override Projectile Shoot(){

		if(Strike == null || SpawnPoint == null)
			return null;

		//looks for any custom spawn point forced externally. otherwise use the standard muzzle
		Vector2 referencePos;
		Quaternion referenceRot;

		if(_customSpawnPoint != null){
			referencePos = _customSpawnPoint.position;
			referenceRot = _customSpawnPoint.localRotation;
		}
		else if(_useOverriddenProp){
			referencePos = _overriddenPos;
			referenceRot = _overriddenRot;
		}
		else{
			referencePos = SpawnPoint.position;
			referenceRot = SpawnPoint.localRotation;
		}

		//Always apply effects from the owner's position. Ignore spawner position but consider rotation
		Projectile bulletInst = Instantiate(Strike, _owner.transform.position, referenceRot) as Projectile;

		if(bulletInst != null){
			bulletInst.Owner = _owner;
			bulletInst.TriggerEffects();
		}

		//The effect is played from the spawn position and with the same rotation
		//Plus, make the effect stay fixed
		if(VisualEffect != null){

            bool facingRight = _owner.GetComponent<HeroController>().IsFacingRight;
			GameObject visualFx = Instantiate(VisualEffect, referencePos, Quaternion.identity) as GameObject;
			visualFx.transform.SetParent(_owner.transform);

            if(facingRight)
                visualFx.transform.localRotation = referenceRot;
            else{
                visualFx.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                visualFx.transform.localRotation = Quaternion.Euler(0, 0, 180-referenceRot.eulerAngles.z);
            }
        }

		return bulletInst;
		
	}
	/*
	*/
}
