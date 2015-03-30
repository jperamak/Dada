using UnityEngine;
using System.Collections;



public class Damageable : MonoBehaviour {

	public float MaxHitpoints = 10.0f;				// hitpoints when undamaged
	public KilledCallback OnDestroy;



    public SoundEffect DamageSound;
    public SoundEffect DestroySound;
	protected float _currentHitpoints;
	protected float _lastTimePlayAudio = 0;
	protected float _playAudioDelay = 0.25f;
	protected GameObject _lastHitFrom;

	
	void Awake () {
		_currentHitpoints = MaxHitpoints;
        DestroySound = DadaAudio.GetSoundEffect(DestroySound);
        DamageSound = DadaAudio.GetSoundEffect(DamageSound);
	}

	public virtual void TakeDamage( float hitpoints ) {
		TakeDamage(hitpoints,null);
	}

	public virtual void TakeDamage( float hitpoints, GameObject dealer) {
		if(_currentHitpoints > 0){
			_currentHitpoints -= hitpoints;
			_lastHitFrom = dealer;

			if(DamageSound != null > 0 && _currentHitpoints > 0 && Time.time > _lastTimePlayAudio+_playAudioDelay){
                DamageSound.PlayEffect();
				_lastTimePlayAudio = Time.time;
			}

			// If there is no hitpoints left, destroy the object.
			if (_currentHitpoints <= 0) {
			//	SendMessage("OnZeroHp", SendMessageOptions.DontRequireReceiver);
				OnDestroyed();
			}
		}
	}

	public virtual void RestoreToMaxHp() {
		_currentHitpoints = MaxHitpoints;
	}

	protected virtual void OnDestroyed() {
        if (DestroySound != null)
            DestroySound.PlayEffect();

		if(OnDestroy != null)
			OnDestroy(this.gameObject,_lastHitFrom);

		Destroy(gameObject);
	}



	            
}
