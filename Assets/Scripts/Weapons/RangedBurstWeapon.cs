using UnityEngine;
using System.Collections;

public class RangedBurstWeapon : RangedWeapon {

	//public int ammoPerBurst = 10;
	public float timePerBurst = 1f; 
	bool firing = false;
	//int shotsInBurstLeft;
	//float nextFireTime;
	public float scatter = 0f;
	float burstStartTime;

	void Update()
	{
		if (firing)
		{

			if (Time.time > (timePerBurst + burstStartTime))
				firing=false;
			else
				ActuallyShoot ();

		}
	}

	protected override Projectile Shoot(){
		firing = true;
		burstStartTime = Time.time;

		_currentBullets--;
		return null;
	}


	Projectile ActuallyShoot() {
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
			bulletInst.Owner = _owner;

			//add velocity instead of force
			if(bulletInst.GetComponent<Rigidbody2D>() != null)
			{
                bulletInst.GetComponent<Rigidbody2D>().velocity = (_referencePoint.right * PushForce);
                bulletInst.GetComponent<Rigidbody2D>().velocity += gameObject.transform.parent.parent.gameObject.GetComponent<Rigidbody2D>().velocity / 2;
				bulletInst.GetComponent<Rigidbody2D>().velocity += new Vector2( Random.Range(-scatter,scatter), Random.Range(-scatter,scatter));
            }
				//bulletInst.GetComponent<Rigidbody2D>().AddForce(_referencePoint.right * PushForce,ForceMode2D.Impulse);

		}

		return bulletInst;
	}



				
}
