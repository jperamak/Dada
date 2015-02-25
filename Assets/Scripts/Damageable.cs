using UnityEngine;
using System.Collections;



abstract public class Damageable : MonoBehaviour {

	public int maxHitpoints = 10;				// hitpoints when undamaged
	protected int currentHitpoints = 10;


	void Awake () {
		currentHitpoints = maxHitpoints;
	}

	public virtual void TakeDamage( int hitpoints ) {
		currentHitpoints -= hitpoints;

		// If there is no hitpoints left, destroy the object.
		if (currentHitpoints <= 0) {
			OnDestroyed();
			return;
		}

	}

	protected virtual void OnDestroyed() {
		Debug.Log("D3");

		Destroy(gameObject);
	}



	            
}
