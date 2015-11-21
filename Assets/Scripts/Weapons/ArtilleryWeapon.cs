using UnityEngine;
using System.Collections;

public class ArtilleryWeapon : RangedWeapon {

	public GameObject aimBar;
	public GameObject emptyAimBar;
	public float pushForceTimeToFull = 5f;

	bool aimMode = false;
	float pushForcePercent = 1f;


	void Update(){
		if (aimMode) {
			pushForcePercent += Time.deltaTime / pushForceTimeToFull;
			if (pushForcePercent > 1.0f)
				pushForcePercent = 1.0f;
			aimBar.transform.localScale = new Vector3(5f*pushForcePercent,5f,5f);
		}
	}

	public override void OnTriggerDown (){
        //if (_currentBullets == 0 && OutOfAmmo != null)
        //    OutOfAmmo.PlayEffect();

		StartAiming();
	}

	public override void OnTriggerUp (){
			StopAiming();
			Shoot ();
	}

	protected override Projectile Shoot(){

		float temp = PushForce;
		PushForce = PushForce * pushForcePercent;
		Projectile bulletInst = base.Shoot();
		PushForce = temp;
    
		return bulletInst;
	}

	void StartAiming()
	{
		aimMode = true;
		aimBar.SetActive(true);
		emptyAimBar.SetActive(true);
		aimBar.transform.localScale.Set (0f,5f,5f);
		pushForcePercent = 0f;
	}

	void StopAiming()
	{
		aimMode = false;
		aimBar.SetActive(false);
		emptyAimBar.SetActive(false);
	}
				
}
