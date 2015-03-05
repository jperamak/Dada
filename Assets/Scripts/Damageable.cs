using UnityEngine;
using System.Collections;

public delegate void Callback();

public class Damageable : MonoBehaviour {

	public float maxHitpoints = 10.0f;				// hitpoints when undamaged
	public float currentHitpoints = 10.0f;
	public Callback Destroyed;

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
		if(Destroyed != null)
			Destroyed();
		Destroy(gameObject);

	}



	            
}
