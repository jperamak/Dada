using UnityEngine;
using System.Collections;



public class Damageable : MonoBehaviour {

	public float maxHitpoints = 10.0f;				// hitpoints when undamaged
	public KilledCallback OnDestroy;

	protected float _currentHitpoints;
	private GameObject _lastHitFrom;



	void Awake () {
		_currentHitpoints = maxHitpoints;
	}

	public virtual void TakeDamage( float hitpoints ) {
		TakeDamage(hitpoints,null);
	}

	public virtual void TakeDamage( float hitpoints, GameObject dealer) {

		if(_currentHitpoints > 0){
			_currentHitpoints -= hitpoints;
			_lastHitFrom = dealer;
			// If there is no hitpoints left, destroy the object.
			if (_currentHitpoints <= 0) {
				OnDestroyed();
			}
		}
	}

	protected virtual void OnDestroyed() {
		if(OnDestroy != null)
			OnDestroy(this.gameObject,_lastHitFrom);
		Destroy(gameObject);
	}



	            
}
