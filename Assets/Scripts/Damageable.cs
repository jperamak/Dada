using UnityEngine;
using System.Collections;



public class Damageable : MonoBehaviour {

	public float maxHitpoints = 10.0f;				// hitpoints when undamaged
	public float currentHitpoints = 10.0f;


	void Awake () {
		currentHitpoints = maxHitpoints;
	}

	public virtual void TakeDamage( float hitpoints ) {
		currentHitpoints -= hitpoints;

		// If there is no hitpoints left, destroy the object.
		if (currentHitpoints <= 0.0) {
			OnDestroyed();
		}

	}

	protected virtual void OnDestroyed() {
		Destroy(gameObject);
	}



	            
}
