using UnityEngine;
using System.Collections;

public abstract class Weapon : MonoBehaviour {


	public float CooldownTime = 0.2f;
	public AudioClip FireSound;
	
	protected GameObject _owner;
	protected Player _player;

	protected float _lastShoot = 0;
	protected Transform _customSpawnPoint;

	public void SetOwner(GameObject owner){
		_owner = owner;
		if(owner != null){
			Hero hero = owner.GetComponent<Hero>();
			if(hero != null)
				_player = hero.PlayerInstance;
		}
	}

	protected abstract void Shoot();

	public virtual void OnTriggerDown (){
		if(Time.time - _lastShoot > CooldownTime){

			if(FireSound != null)
				DadaGame.PlaySound(FireSound);

			Shoot();
			_lastShoot = Time.time;
		}
	}

	public virtual void OnTriggerUp(){
		_customSpawnPoint = null;
	}

	public virtual void OnTriggerDown(Transform customSpawnPoint){
		_customSpawnPoint = customSpawnPoint;
		OnTriggerDown();
	}
}
