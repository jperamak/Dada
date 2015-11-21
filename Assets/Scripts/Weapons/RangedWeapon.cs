using UnityEngine;
using System.Collections;

public class RangedWeapon : Weapon {

	public Projectile Bullet;
	public int MaxBullets = 10;
	public float RechargeEvery = 0.5f;
	public float PushForce = 20.0f;
	public bool adjustablePuishForce = false;
	public SoundEffect OutOfAmmo;


	protected int _currentBullets;

	void Start(){
		if(SpawnPoint == null)
			SpawnPoint = transform.FindChild("Spawner");
		_currentBullets = MaxBullets;
		if(RechargeEvery > 0)
			InvokeRepeating("Recharge",0,RechargeEvery);
        OutOfAmmo = DadaAudio.GetSoundEffect(OutOfAmmo);
	}

	public override void OnTriggerDown (){
        if (_currentBullets == 0 && OutOfAmmo != null)
            OutOfAmmo.PlayEffect();

		base.OnTriggerDown ();
	}

	protected override Projectile Shoot(){

		if(_currentBullets <= 0 || Bullet == null || SpawnPoint == null)
			return null;

		//looks for any custom spawn point forced externally. otherwise use the standard muzzle
		Transform _referencePoint = _customSpawnPoint != null ? _customSpawnPoint : SpawnPoint;

		Projectile bulletInst = Instantiate(Bullet, _referencePoint.position, _referencePoint.rotation) as Projectile;

		//HACK: Prevents collision with owner and some onjects
		GameObject[] collObs = GetAboutCollidingObjects.WithinRadius(bulletInst.gameObject, 0.7f);
		foreach (GameObject co in collObs) {
			if (co.tag == "CanShootThrough")
				GetAboutCollidingObjects.IgnoreCollisionBetween( bulletInst.gameObject, co);
		}
		GetAboutCollidingObjects.IgnoreCollisionBetween( bulletInst.gameObject, _owner);

		//apply force to created projectile
		if(bulletInst != null){
			_currentBullets--;
			bulletInst.Owner = _owner;

			//add velocity instead of force
			if(bulletInst.GetComponent<Rigidbody2D>() != null)
			{
                bulletInst.GetComponent<Rigidbody2D>().velocity = (_referencePoint.right * PushForce);
                bulletInst.GetComponent<Rigidbody2D>().velocity += gameObject.transform.parent.parent.gameObject.GetComponent<Rigidbody2D>().velocity / 2;
            }
				//bulletInst.GetComponent<Rigidbody2D>().AddForce(_referencePoint.right * PushForce,ForceMode2D.Impulse);

		}

		return bulletInst;
	}

	protected void Recharge(){
		if(_currentBullets < MaxBullets)
			_currentBullets++;
	}


				
}
