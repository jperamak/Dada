﻿using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
	public Rigidbody2D rocket;				// Prefab of the rocket.
	public float speed = 20f;				// The speed the rocket will fire at.
    public float cooldown;



	private PlayerControl playerCtrl;		// Reference to the PlayerControl script.
	private Animator anim;					// Reference to the Animator component.
    private float time;

	void Awake()
	{
		// Setting up the references.
		anim = transform.root.gameObject.GetComponent<Animator>();
		playerCtrl = transform.root.GetComponent<PlayerControl>();
        time = Time.fixedTime - cooldown;
	}


	void Update ()
	{
		// If the fire button is pressed...
        time += Time.deltaTime;

		if(playerCtrl.controller.GetButtonDown(VirtualKey.SHOOT) && time > cooldown)
		{
            time = 0;
			// ... set the animator Shoot trigger parameter and play the audioclip.
			anim.SetTrigger("Shoot");
			audio.Play();

			// If the player is facing right...
			if(playerCtrl.facingRight)
			{
				// ... instantiate the rocket facing right and set it's velocity to the right. 
				Rigidbody2D bulletInstance = Instantiate(rocket, transform.position, Quaternion.Euler(new Vector3(0,0,0))) as Rigidbody2D;
                bulletInstance.GetComponent<Rocket>().player = playerCtrl;

				bulletInstance.velocity = new Vector2(speed, 0);
			}
			else
			{
				// Otherwise instantiate the rocket facing left and set it's velocity to the left.
				Rigidbody2D bulletInstance = Instantiate(rocket, transform.position, Quaternion.Euler(new Vector3(0,0,180f))) as Rigidbody2D;
                bulletInstance.GetComponent<Rocket>().player = playerCtrl;
				bulletInstance.velocity = new Vector2(-speed, 0);
			}
		}
	}
}
