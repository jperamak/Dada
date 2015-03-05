using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

    public GameObject ParticlePrefab;
    public int NumOfRays = 15;

    public void Explode(PlayerControl source, float radius, float explosionForce, int damage)
    {
        Vector2 dir;
        Vector2 position = new Vector2(transform.position.x, transform.position.y);

		float divider = NumOfRays;
        float invDiv = 1 / divider;
        float step = 360f * invDiv;

        for (int i = 0; i < divider; i++)
        {
            dir = Quaternion.AngleAxis(step*0.5f + step*i,Vector3.forward)*Vector2.up;

            GameObject ep = Instantiate(ParticlePrefab) as GameObject;
            ep.rigidbody2D.AddForce(dir * explosionForce*100*radius);
            ep.transform.position = transform.position;
            var ep2 = ep.GetComponent<ExplosionParticle>();
            ep2.source = source;
            ep2.damage = damage * invDiv;
            

			int numTimesDamped = 0; // How many times the ray has hit a object that dampens the explosion

            // hitting only background tiles, foreground dampens
            RaycastHit2D[] hits = Physics2D.LinecastAll(position, position + dir * radius, LayerMask.GetMask(new string[] { "Ground", "BackgroundBlock" }));
            foreach (RaycastHit2D hit in hits)
            {
                // Solid objects in foreground dampen the explosion
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    numTimesDamped++;
                }
                
                if (hit.rigidbody)
                    hit.rigidbody.AddForce(dir * 100 * explosionForce);
                
                // If the hit object is damageable...
                if (hit.collider.gameObject.GetComponent<Damageable>() != null && hit.transform.gameObject.layer != LayerMask.NameToLayer("Ground"))
                {
					// calculate the basic damage
					float rayDamage = (float)damage * invDiv;
					// apply dampening
					if (numTimesDamped > 0)
						rayDamage = rayDamage * Mathf.Pow(0.75f, numTimesDamped);
                    // inflict the damage
                    hit.collider.gameObject.GetComponent<Damageable>().TakeDamage(rayDamage);
                }

                // If a player is hit before the explosion is dampened... 
                if (hit.collider.gameObject.tag == "Player" && numTimesDamped == 0)
				{	
                    // Instantiate the explosion and destroy the rocket.
                    PlayerHealth pH = hit.collider.gameObject.GetComponent<PlayerHealth>();
                    pH.TakeDamage(source);
                }
            }
        }
    }
}
