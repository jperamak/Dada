using UnityEngine;
using System.Collections;

public class ScatteredExplosion : ParticleExplosion {


	protected override void Execute (){
		if (ExplosionSound != null)
			ExplosionSound.PlayEffect();
		Vector2 dir;		

		Vector2 velocity = GetComponent<Rigidbody2D>().velocity;
		
		for (int i = 0; i < NumOfParticles; i++)
		{
			dir = Random.insideUnitCircle;
			Projectile ep = Instantiate(ExplosionParticles) as Projectile;
			ep.Owner = Owner;
			
			//edit properties of new projectiles
			Damage[] dmg = ep.GetComponents<Damage>();
			if(dmg != null && dmg.Length > 0){
				for(int j=0; j< dmg.Length; j++){
					dmg[j].DamageOwner = DamageOwner;
				}
			}

			Rigidbody2D rbody = ep.GetComponent<Rigidbody2D>();
			float lifetime = ParticleLifetime + Random.Range(-1.0f, 1.0f);
			ep.gameObject.layer = LayerMask.NameToLayer("ExplosionParticle");
			rbody.AddForce(dir * ExplosionForce * 100);
			rbody.velocity += velocity*0.7f;
			ep.transform.position = transform.position;
			Destroy(ep.gameObject,lifetime);
		}
	}
}
