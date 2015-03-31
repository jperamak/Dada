using UnityEngine;
using System.Collections;

public class ShockWave : Damage {
	
	public float Radius;
	public int NumOfRays = 15;
	public LayerMask InteractWith;
	public float ExplosionPushForce;
	
	protected override void Execute (){
		
		Vector2 dir;
		Vector2 position = new Vector2(transform.position.x, transform.position.y);
		
		
		float divider = NumOfRays;
		float invDiv = 1 / divider;
		float step = 360f * invDiv;
		
		for (int i = 0; i < divider; i++)
		{
			dir = Quaternion.AngleAxis(step*0.5f + step*i,Vector3.forward)*Vector2.up;
			int numTimesDamped = 0; // How many times the ray has hit a object that dampens the explosion
			
		// hitting only background tiles, foreground dampens
		//	RaycastHit2D[] hits = Physics2D.RaycastAll(position, position + dir * Radius, InteractWith);
		RaycastHit2D[] hits = Physics2D.RaycastAll(position, dir, Radius, InteractWith);
		for(int j=0;j<hits.Length;j++){
				
			// Solid objects in foreground dampen the explosion
			if (hits[j].transform.gameObject.layer == LayerMask.NameToLayer("Ground") ||
				   hits[j].transform.gameObject.layer == LayerMask.NameToLayer("ForegroundObject")) 
			{ 
					numTimesDamped++;
			}
				
			if (hits[j].rigidbody)
				hits[j].rigidbody.AddForce(dir * 100 * ExplosionPushForce);
				
			Damageable damageable = hits[j].collider.gameObject.GetComponent<Damageable>();
				
			if (damageable == null)
				continue;
				
			if (hits[j].collider.gameObject.tag == "Player" && numTimesDamped > 0)
				continue;
				
			if (hits[j].collider.gameObject == Owner)
				   continue;


			if (hits[j].transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
				continue;
				
				
			// calculate the basic damage
			float rayDamage = (float)DamageAmount * invDiv;
			// apply dampening
			if (numTimesDamped > 0)
				rayDamage = rayDamage * Mathf.Pow(0.75f, numTimesDamped);
			// inflict the damage
			damageable.TakeDamage(rayDamage, Owner);
				
		}
		}
		
	}
	
}
