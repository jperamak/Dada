using UnityEngine;
using System.Collections;



public class Damageable : MonoBehaviour {

	public int maxHitpoints = 10;				// hitpoints when undamaged
	public int hitpointsLightDamage = 8;		// hitpoints when lightly damaged sprite is applied 
	public int hitpointsHeavyDamage = 4;		// hitpoints when heavily damaged sprite is applied

	public Sprite spriteLightDamage = null;		// sprite for lightly damaged object
	public Sprite spriteHeavyDamage = null;		// sprite for heavily damaged object

	public GameObject leftWhenDestroyed = null;  // When hitpoints reach zero, the gameObject is replaced with this one

	private int hitpointsLeft = 10;


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

		if (leftWhenDestroyed != null) {
			GameObject ruin = Instantiate(leftWhenDestroyed, transform.position, transform.rotation) as GameObject;
			ruin.transform.localScale = transform.localScale;

		}

		Destroy(gameObject);
	}



	            
}
