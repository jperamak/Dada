using UnityEngine;
using System.Collections;



public class DamageableBlock : Damageable {

//	public int maxHitpoints = 10;				// hitpoints when undamaged
	public int hitpointsLightDamage = 8;		// hitpoints when lightly damaged sprite is applied 
	public int hitpointsHeavyDamage = 4;		// hitpoints when heavily damaged sprite is applied

	public Sprite spriteLightDamage = null;		// sprite for lightly damaged object
	public Sprite spriteHeavyDamage = null;		// sprite for heavily damaged object

	public GameObject leftWhenDestroyed = null;  // When hitpoints reach zero, the gameObject is replaced with this one

//	private int currentHitpoints = 10;


//	void Awake () {
//		hitpointsLeft = maxHitpoints;
//	}

	public override void TakeDamage( int hitpoints ) {

		base.TakeDamage( hitpoints ); // also destroys object on 0 hp

		SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();

		// Change sprite according to hitpoints left
		if (currentHitpoints <= hitpointsHeavyDamage && spriteHeavyDamage != null)
			sr.sprite = spriteHeavyDamage;
		else if (currentHitpoints <= hitpointsLightDamage && spriteLightDamage != null)
			sr.sprite = spriteLightDamage;

		if (currentHitpoints <= 0 && leftWhenDestroyed != null) {
			LeaveRuins();
		}

		
		
	}

	private void LeaveRuins() {

			GameObject ruin = Instantiate(leftWhenDestroyed, transform.position, transform.rotation) as GameObject;
			ruin.transform.localScale = transform.localScale;

	}



	            
}
