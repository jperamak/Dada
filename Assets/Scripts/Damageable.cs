using UnityEngine;
using System.Collections;



public class Damageable : MonoBehaviour {

	public int maxHitpoints = 10;				// hitpoints when undamaged
	public int hitpointsLightDamage = 8;		// hitpoints when lightly damaged sprite is applied 
	public int hitpointsHeavyDamage = 4;		// hitpoints when heavily damaged sprite is applied

	private int hitpointsLeft = 10;

	public Sprite spriteLightDamage = null;		// sprite for lightly damaged object
	public Sprite spriteHeavyDamage = null;		// sprite for heavily damaged object

	void Awake () {
		hitpointsLeft = maxHitpoints;
	}

	public void TakeDamage( int hitpoints ) {

		hitpointsLeft -= hitpoints;

		// If there is no hitpoints left, destroy the object.
		if (hitpointsLeft <= 0) {
			OnDestroyed();
			return;
		}

		SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();

		// Change sprite according to hitpoints left
		if (hitpointsLeft <= hitpointsHeavyDamage && spriteHeavyDamage != null)
			sr.sprite = spriteHeavyDamage;
		else if (hitpointsLeft <= hitpointsLightDamage && spriteLightDamage != null)
			sr.sprite = spriteLightDamage;

	}

	private void OnDestroyed() {
		Destroy(gameObject);
	}



	            
}
