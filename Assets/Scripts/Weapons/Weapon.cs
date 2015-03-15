using UnityEngine;
using System.Collections;

public abstract class Weapon : MonoBehaviour {


	public float CooldownTime = 0.2f;
	public GameObject VisualEffect;
	public AudioClip[] FireSound;
	
	protected GameObject _owner;
	protected Player _player;

	protected float _lastShoot = 0;
	protected Transform _customSpawnPoint;

	//used for spawn at any position/direction
	protected Vector2 _overriddenPos;
	protected Quaternion _overriddenRot;
	protected bool _useOverriddenProp = false;

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

			if(FireSound != null && FireSound.Length > 0)
				DadaAudio.PlayRandom(FireSound);


			Shoot();
			_lastShoot = Time.time;
		}
	}

	public virtual void OnTriggerUp(){
		_customSpawnPoint = null;
		_useOverriddenProp = false;
	}

	public virtual void OnTriggerDown(Transform customSpawnPoint){
		_customSpawnPoint = customSpawnPoint;
		OnTriggerDown();
	}

	public virtual void OnTriggerDown(Vector2 position, Quaternion rotation){
		_overriddenPos = position;
		_overriddenRot = rotation;
		_useOverriddenProp = true;
		OnTriggerDown();
	}
}
