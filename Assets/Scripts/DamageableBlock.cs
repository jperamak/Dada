using UnityEngine;
using System.Collections;



public class DamageableBlock : Damageable {

	public float hitpointsLightDamage = 8.0f;		// hitpoints when lightly damaged sprite is applied 
	public float hitpointsHeavyDamage = 4.0f;		// hitpoints when heavily damaged sprite is applied

	public Sprite spriteLightDamage = null;		// sprite for lightly damaged object
	public Sprite spriteHeavyDamage = null;		// sprite for heavily damaged object

	public GameObject leftWhenDestroyed = null;  // When hitpoints reach zero, the gameObject is replaced with this one

	public bool damageFromCollisions;

	public override void TakeDamage( float hitpoints, PlayerControl dealer ) {

		base.TakeDamage( hitpoints, dealer ); // also destroys object on 0 hp

		SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();

		// Change sprite according to hitpoints left
		if (currentHitpoints <= hitpointsHeavyDamage && spriteHeavyDamage != null)
			sr.sprite = spriteHeavyDamage;
		else if (currentHitpoints <= hitpointsLightDamage && spriteLightDamage != null)
			sr.sprite = spriteLightDamage;

		if (currentHitpoints <= 0.0f && leftWhenDestroyed != null) {
			LeaveRuins();
		}

		
		
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (!damageFromCollisions)
			return;

		if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || 
		    other.gameObject.layer == LayerMask.NameToLayer("BackgroundBlock") ||
		    other.gameObject.layer == LayerMask.NameToLayer("Rubble") 
		    )
		{
			Rigidbody2D o = other.gameObject.GetComponent<Rigidbody2D>();
			
			if (o != null && other.relativeVelocity.magnitude > 15f)
				TakeDamage(1000, null);
		}
	}

	private void LeaveRuins() {

			GameObject ruin = Instantiate(leftWhenDestroyed, transform.position, transform.rotation) as GameObject;
			ruin.transform.localScale = transform.localScale;

	}



	            
}
