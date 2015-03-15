using UnityEngine;
using System.Collections;

public class RangedWeapon : Weapon {

	public Projectile Bullet;
	public int MaxBullets = 10;
	public float RechargeEvery = 0.5f;
	public float PushForce = 20.0f;
	public AudioClip OutOfAmmo;

	protected Transform _spawnPoint;
	protected int _currentBullets;

	void Start(){
		_spawnPoint = transform.FindChild("Spawner");
		_currentBullets = MaxBullets;
		InvokeRepeating("Recharge",0,RechargeEvery);
	}

	public override void OnTriggerDown (){
		if(_currentBullets == 0 && OutOfAmmo != null)
			DadaAudio.PlaySound(OutOfAmmo);

		base.OnTriggerDown ();
	}

	protected override void Shoot(){

		if(_currentBullets <= 0 || Bullet == null || _spawnPoint == null)
			return;

		//looks for any custom spawn point forced externally. otherwise use the standard muzzle
		Transform _referencePoint = _customSpawnPoint != null ? _customSpawnPoint : _spawnPoint;

		Projectile bulletInst = Instantiate(Bullet, _referencePoint.position, _referencePoint.rotation) as Projectile;

		//apply force to created projectile
		if(bulletInst != null){
			_currentBullets--;
			bulletInst.Owner = _owner;

			//add velocity instead of force
			if(bulletInst.GetComponent<Rigidbody2D>() != null)
				bulletInst.GetComponent<Rigidbody2D>().velocity = (_referencePoint.right * PushForce);

				//bulletInst.GetComponent<Rigidbody2D>().AddForce(_referencePoint.right * PushForce,ForceMode2D.Impulse);

		}
	}

	protected void Recharge(){
		if(_currentBullets < MaxBullets)
			_currentBullets++;
	}
}
