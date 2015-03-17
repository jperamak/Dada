﻿using UnityEngine;
using System.Collections;



public class DamageableBlock : Damageable {

	public float hitpointsLightDamage = 8.0f;		// hitpoints when lightly damaged sprite is applied 
	public float hitpointsHeavyDamage = 4.0f;		// hitpoints when heavily damaged sprite is applied

	public Sprite spriteLightDamage = null;		// sprite for lightly damaged object
	public Sprite spriteHeavyDamage = null;		// sprite for heavily damaged object

	public GameObject leftWhenDestroyed = null;  // When hitpoints reach zero, the gameObject is replaced with this one

	public bool damageFromCollisions;

	private Sprite _spriteNoDamage = null;

	void Start() {
		SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
		if (sr != null)
			_spriteNoDamage = sr.sprite;
	}

	public override void TakeDamage( float hitpoints, GameObject dealer ) {

		base.TakeDamage( hitpoints, dealer ); // also destroys object on 0 hp

		UpdateSprite();

		if (_currentHitpoints <= 0.0f && leftWhenDestroyed != null) {
			LeaveRuins();
		}

		
		
	}

	public override void RestoreToMaxHp() {
		base.RestoreToMaxHp();
		UpdateSprite();
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

	// change sprite according to how damaged the object is
	private void UpdateSprite() {
		SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
		
	
		if (_currentHitpoints <= hitpointsHeavyDamage && spriteHeavyDamage != null)
			sr.sprite = spriteHeavyDamage;
		else if (_currentHitpoints <= hitpointsLightDamage && spriteLightDamage != null)
			sr.sprite = spriteLightDamage;
		else
			sr.sprite = _spriteNoDamage;
	}


	private void LeaveRuins() {

			GameObject ruin = Instantiate(leftWhenDestroyed, transform.position, transform.rotation) as GameObject;
			ruin.transform.localScale = transform.localScale;

	}



	            
}
