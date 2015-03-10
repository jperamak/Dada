using UnityEngine;
using System.Collections;

public delegate void Callback(PlayerControl dealer);

public class Damageable : MonoBehaviour {

	public float maxHitpoints = 10.0f;				// hitpoints when undamaged
	public float currentHitpoints = 10.0f;
	public bool destroyOnZeroHp = true;
	public Callback Destroyed;

	void Awake () {
		currentHitpoints = maxHitpoints;
	}

	// If dealer == null, no one is responsible
	public virtual void TakeDamage( float hitpoints, PlayerControl dealer ) {
		currentHitpoints -= hitpoints;

		// If there is no hitpoints left, destroy the object.
		if (currentHitpoints <= 0.0) {
			OnDestroyed(dealer);
		}

	}

	protected virtual void OnDestroyed(PlayerControl dealer) {
		if(Destroyed != null)
			Destroyed(dealer);

		if (destroyOnZeroHp)
			Destroy(gameObject);

	}



	            
}
