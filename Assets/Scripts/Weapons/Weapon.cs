using UnityEngine;
using System.Collections;

public abstract class Weapon : MonoBehaviour {


	public float CooldownTime = 0.2f;
	public GameObject VisualEffect;
    public SoundEffect FireSound;
	public Transform SpawnPoint;
	
	protected GameObject _owner;
	protected Player _player;

	protected float _lastShoot = 0;
	protected Transform _customSpawnPoint;

	//used for spawn at any position/direction
	protected Vector2 _overriddenPos;
	protected Quaternion _overriddenRot;
	protected bool _useOverriddenProp = false;

    void Awake()
    {
        FireSound = DadaAudio.GetSoundEffect(FireSound);
    }

	public void SetOwner(GameObject owner){
		_owner = owner;
		if(owner != null){
			Hero hero = owner.GetComponent<Hero>();
			if(hero != null)
				_player = hero.PlayerInstance;
		}
	}

	protected abstract Projectile Shoot();

	public virtual void OnTriggerDown (){
		if(Time.time - _lastShoot > CooldownTime){

            if (FireSound != null)
                FireSound.PlayEffect();


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
