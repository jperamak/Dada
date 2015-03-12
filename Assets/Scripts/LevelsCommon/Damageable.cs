using UnityEngine;
using System.Collections;



public class Damageable : MonoBehaviour {

	public float MaxHitpoints = 10.0f;				// hitpoints when undamaged
	public KilledCallback OnDestroy;

	public AudioClip[] DamageSound;
	public AudioClip DestroySound;

	protected float _currentHitpoints;
	protected float _lastTimePlayAudio = 0;
	protected float _playAudioDelay = 0.25f;
	private GameObject _lastHitFrom;

	
	void Awake () {
		_currentHitpoints = MaxHitpoints;
	}

	public virtual void TakeDamage( float hitpoints ) {
		TakeDamage(hitpoints,null);
	}

	public virtual void TakeDamage( float hitpoints, GameObject dealer) {

		if(_currentHitpoints > 0){
			_currentHitpoints -= hitpoints;
			_lastHitFrom = dealer;

			if(DamageSound.Length > 0 && _currentHitpoints > 0 && Time.time > _lastTimePlayAudio+_playAudioDelay){
				DadaAudio.PlayRandom(DamageSound);
				_lastTimePlayAudio = Time.time;
			}

			// If there is no hitpoints left, destroy the object.
			if (_currentHitpoints <= 0) {
				OnDestroyed();
			}
		}
	}

	protected virtual void OnDestroyed() {
		if(DestroySound != null)
			DadaAudio.PlaySound(DestroySound);

		if(OnDestroy != null)
			OnDestroy(this.gameObject,_lastHitFrom);
		Destroy(gameObject);
	}



	            
}
