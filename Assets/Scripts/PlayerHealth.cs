﻿using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{	
	public float health = 100f;					// The player's health.
	public float repeatDamagePeriod = 2f;		// How frequently the player can be damaged.
	public AudioClip[] ouchClips;				// Array of clips to play when the player is damaged.
	public float hurtForce = 10f;				// The force with which the player is pushed when hurt.
	public float damageAmount = 10f;			// The amount of damage to take when enemies touch the player

	private float _lastHitTime;					// The time at which the player was last hit.
	private Vector3 _healthScale;				// The local scale of the health bar initially (with full health).
	private PlayerControl _playerControl;		// Reference to the PlayerControl script.
	private Animator _anim;						// Reference to the Animator on the player


	void Awake ()
	{
		// Setting up references.
		_playerControl = GetComponent<PlayerControl>();
		_anim = GetComponent<Animator>();

		// Getting the intial scale of the healthbar (whilst the player has full health).
	}


	void OnCollisionEnter2D (Collision2D col)
	{
		// If the colliding gameobject is an Enemy...
    	if(col.gameObject.tag == "Enemy")
		{
			// ... and if the time exceeds the time of the last hit plus the time between hits...
			if (Time.time > _lastHitTime + repeatDamagePeriod) 
			{
				// ... and if the player still has health...
			    // ... take damage and reset the lastHitTime.
			    TakeDamage(null); 
			    _lastHitTime = Time.time; 
				
			}
		}
	}


	public void TakeDamage (PlayerControl shooter)
	{
		// Make sure the player can't jump.
		_playerControl.jump = false;

        if (health <= 0)
            return;
		// Reduce the player's health by 10.
		health -= damageAmount;

		// Play a random clip of the player getting hurt.
		int i = Random.Range (0, ouchClips.Length);
		AudioSource.PlayClipAtPoint(ouchClips[i], transform.position);

        if (health <= 0f)
        {
            if (_playerControl.Equals(shooter))
                GameObject.Find("LevelManager").GetComponent<PlayerSpawner>().AddPoint(_playerControl.controller.Number, 1);
            else if (shooter != null)
                GameObject.Find("LevelManager").GetComponent<PlayerSpawner>().AddPoint(shooter.controller.Number, -1);
            // Find all of the colliders on the gameobject and set them all to be triggers.
            Collider2D[] cols = GetComponents<Collider2D>();
            foreach (Collider2D c in cols)
            {
                c.isTrigger = true;
            }

            // Move all sprite parts of the player to the front
            SpriteRenderer[] spr = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer s in spr)
            {
                s.sortingLayerName = "UI";
            }

            // ... disable user Player Control script
            _playerControl.enabled = false;

            // ... disable the Gun script to stop a dead guy shooting a nonexistant bazooka
            GetComponentInChildren<Gun>().enabled = false;

            // ... Trigger the 'Die' animation state
            _anim.SetTrigger("Die");
            Invoke("Die", 2);
        }
	}

    void Die()
    {
        _playerControl.Die();
    }

}
