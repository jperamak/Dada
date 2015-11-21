using UnityEngine;
using System.Collections;

public class RangedWeapon : Weapon {

	public Projectile Bullet;
	public int MaxBullets = 10;
	public float RechargeEvery = 0.5f;
	public float PushForce = 20.0f;
	public SoundEffect OutOfAmmo;
	public GameObject aimBar;
	public GameObject emptyAimBar;
	public bool adjustablePushForce = false;
	public float pushForceTimeToFull = 5f;

	bool aimMode = false;
	float pushForcePercent = 1f;

    protected int _currentBullets;

	public int currentBullets
    {
        get { return _currentBullets; }
    }

	void Start(){
		if(SpawnPoint == null)
			SpawnPoint = transform.FindChild("Spawner");
		_currentBullets = MaxBullets;
		if(RechargeEvery > 0)
			InvokeRepeating("Recharge",0,RechargeEvery);
        OutOfAmmo = DadaAudio.GetSoundEffect(OutOfAmmo);
	}

	void Update(){
		if (aimMode) {
			pushForcePercent += Time.deltaTime / pushForceTimeToFull;
			if (pushForcePercent > 1.0f)
				pushForcePercent = 1.0f;
			aimBar.transform.localScale = new Vector3(5f*pushForcePercent,5f,5f);
		}
	}

	public override void OnTriggerDown (){
        if (_currentBullets == 0 && OutOfAmmo != null)
            OutOfAmmo.PlayEffect();

		if (adjustablePushForce){
			StartAiming();
		}
		else {
			base.OnTriggerDown();
		}

	}

	public override void OnTriggerUp (){
		if (aimMode){
			StopAiming();
			Shoot ();
		}
		base.OnTriggerUp();
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
                bulletInst.GetComponent<Rigidbody2D>().velocity = (_referencePoint.right * PushForce * pushForcePercent);
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
