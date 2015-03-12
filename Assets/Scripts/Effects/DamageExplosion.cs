using UnityEngine;
using System.Collections;

public class DamageExplosion : Damage {

	public float Radius;
	public float ExplosionForce;
	public int NumOfRays = 15;
	public float ParticleLifetime = 0.2f;
	public Projectile ExplosionParticles;

	protected override void Execute (){
		
		Vector2 dir;
		Vector2 position = new Vector2(transform.position.x, transform.position.y);
		
		float divider = NumOfRays;
		float invDiv = 1 / divider;
		float step = 360f * invDiv;
		
		for (int i = 0; i < divider; i++)
		{
			dir = Quaternion.AngleAxis(step*0.5f + step*i,Vector3.forward)*Vector2.up;
			
			Projectile ep = Instantiate(ExplosionParticles) as Projectile;
			ep.Owner = Owner;

			//edit properties of new projectiles
			Damage[] dmg = ep.GetComponents<Damage>();
			if(dmg != null && dmg.Length > 0){
				for(int j=0; j< dmg.Length; j++){
					dmg[j].DamageOwner = DamageOwner;
					dmg[j].DamageAmount = DamageAmount * invDiv;
				}
			}

			ep.gameObject.layer = LayerMask.NameToLayer("ExplosionParticle");
			ep.GetComponent<Rigidbody2D>().AddForce(dir * ExplosionForce*100*Radius);
			ep.transform.position = transform.position;
			Destroy(ep.gameObject,ParticleLifetime);


			int numTimesDamped = 0; // How many times the ray has hit a object that dampens the explosion
			
			// hitting only background tiles, foreground dampens
			RaycastHit2D[] hits = Physics2D.LinecastAll(position, position + dir * Radius, LayerMask.GetMask(new string[] { "Ground", "BackgroundBlock" }));
			foreach (RaycastHit2D hit in hits)
			{
				
				// Solid objects in foreground dampen the explosion
				if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
				{
					numTimesDamped++;
				}
				
				if (hit.rigidbody)
					hit.rigidbody.AddForce(dir * 100 * ExplosionForce);
				
				Damageable damageable = hit.collider.gameObject.GetComponent<Damageable>();
				
				if (damageable == null)
					continue;
				
				if (hit.collider.gameObject.tag == "Player" && numTimesDamped > 0)
					continue;
				
				if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
					continue;
				
				
				// calculate the basic damage
				float rayDamage = (float)DamageAmount * invDiv;
				// apply dampening
				if (numTimesDamped > 0)
					rayDamage = rayDamage * Mathf.Pow(0.75f, numTimesDamped);
				// inflict the damage
				damageable.TakeDamage(rayDamage, Owner);
				
				
				/*// If a player is hit before the explosion is dampened... 
				if (hit.collider.gameObject.tag == "Player" && numTimesDamped == 0)
				{	
					// Instantiate the explosion and destroy the rocket.
					PlayerHealth pH = hit.collider.gameObject.GetComponent<PlayerHealth>();
					pH.TakeDamage(source);
				}*/
			}
		}
		
	}
	
}
