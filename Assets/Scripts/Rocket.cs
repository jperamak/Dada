﻿using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour 
{
	public GameObject explosion;		// Prefab of explosion effect.
    public float radius;
    [HideInInspector]
    public PlayerControl player;

	void Start () 
	{
		// Destroy the rocket after 2 seconds if it doesn't get destroyed before then.
		Destroy(gameObject, 2);
	}


	void OnExplode()
	{
		// Create a quaternion with a random rotation in the z-axis.
		Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

		// Instantiate the explosion where the rocket is with the random rotation.
		Instantiate(explosion, transform.position, randomRotation);
	}
	
	void OnTriggerEnter2D (Collider2D c) 
	{

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask(new string[] {"Enemy", "Player", "Ground"}));
        foreach (Collider2D col in hits)
        {
            Debug.Log(c);
            // If the hit object is damageable...
            if (col.gameObject.GetComponent<Damageable>() != null)
            {
                // ...give it 10 hitpointd of damage
                col.gameObject.GetComponent<Damageable>().TakeDamage(10);
            }

            // If it hits an enemy...
            if (col.tag == "Enemy")
            {
                // ... find the Enemy script and call the Hurt function.
                col.gameObject.GetComponent<Enemy>().Hurt();

                // Call the explosion instantiation.
                OnExplode();

                // Destroy the rocket.
                Destroy(gameObject);
            }
            // Otherwise if it hits a bomb crate...
            else if (col.tag == "BombPickup")
            {
                // ... find the Bomb script and call the Explode function.
                col.gameObject.GetComponent<Bomb>().Explode();

                // Destroy the bomb crate.
                Destroy(col.transform.root.gameObject);

                // Destroy the rocket.
                Destroy(gameObject);
            }
            // Otherwise if the player manages to shoot himself...
            else if (col.gameObject.tag == "Player")
            {

                // Instantiate the explosion and destroy the rocket.
                OnExplode();
                PlayerHealth pH = col.gameObject.GetComponent<PlayerHealth>();
                pH.TakeDamage(gameObject.transform, player);
                Destroy(gameObject);

            }
            else if (col.gameObject.tag != "Player")
            {
                OnExplode();
                Destroy(gameObject);
            }
        }
	}
}
