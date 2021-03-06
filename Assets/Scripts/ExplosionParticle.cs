﻿using UnityEngine;
using System.Collections;

public class ExplosionParticle : MonoBehaviour {

	// Use this for initialization
    public float damage;
    public GameObject source;

	void Start () {
        Destroy(gameObject, 0.2f);
	}

	void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.gameObject.GetComponent<Damageable>() != null)
        {
            // calculate the basic damage
            other.collider.gameObject.GetComponent<Damageable>().TakeDamage(damage, source);

        }

		/*
        // If a player is hit before the explosion is dampened... 
        if (other.collider.gameObject.tag == "Player")
        {
            // Instantiate the explosion and destroy the rocket.
            PlayerHealth pH = other.gameObject.GetComponent<PlayerHealth>();
            pH.TakeDamage(source);
        }*/
    }
}
