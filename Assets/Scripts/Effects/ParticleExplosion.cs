﻿using UnityEngine;
using System.Collections;

public class ParticleExplosion : Damage {
	
	public float ExplosionForce;
	public float ParticleLifetime = 0.2f;
	public LayerMask InteractWith;
	public Projectile ExplosionParticles;
	public int NumOfParticles;
    public SoundEffect ExplosionSound;

    void Start()
    {
        ExplosionSound = DadaAudio.GetSoundEffect(ExplosionSound);
    }
	protected override void Execute (){
        if (ExplosionSound != null)
            ExplosionSound.PlayEffect();
		Vector2 dir;		
		
		float divider = NumOfParticles;
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

			// Every other particle is fast, every other is slow
			if (i%2==0)
				ep.GetComponent<Rigidbody2D>().AddForce(dir * ExplosionForce * 100);
			else
				ep.GetComponent<Rigidbody2D>().AddForce(dir * ExplosionForce * 50);

			ep.transform.position = transform.position;
			Destroy(ep.gameObject,ParticleLifetime);
		}
	}
}
