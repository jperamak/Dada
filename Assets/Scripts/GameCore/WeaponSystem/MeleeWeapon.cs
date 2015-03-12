using UnityEngine;
using System.Collections;

public class MeleeWeapon : Weapon {

	public Projectile Strike;

	protected Transform _spawnPoint;


	void Start(){
		_spawnPoint = transform.FindChild("Spawner");
	}

	protected override void Shoot(){

		if(Strike == null || _spawnPoint == null)
			return;
		

		//looks for any custom spawn point forced externally. otherwise use the standard muzzle
		Transform _referencePoint = _customSpawnPoint != null ? _customSpawnPoint : _spawnPoint;
		
		Projectile bulletInst = Instantiate(Strike, _referencePoint.position, _referencePoint.rotation) as Projectile;

		if(bulletInst != null)
			bulletInst.Owner = _owner;

		
	}
	/*
	*/
}
