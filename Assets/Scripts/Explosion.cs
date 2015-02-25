using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	// Use this for initialization
	public void Explode (PlayerControl source, float radius) {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask(new string[] { "Enemy", "Player", "Ground", "BackgroundBlock" }));
        foreach (Collider2D col in hits)
        {
            // If the hit object is damageable...
            if (col.gameObject.GetComponent<Damageable>() != null)
            {
                // ...give it 10 hitpointd of damage
                col.gameObject.GetComponent<Damageable>().TakeDamage(10);
            }

            // Otherwise if the player manages to shoot himself...
            else if (col.gameObject.tag == "Player")
            {

                // Instantiate the explosion and destroy the rocket.
                PlayerHealth pH = col.gameObject.GetComponent<PlayerHealth>();
                pH.TakeDamage(gameObject.transform, source);
                //OnExplode();
            }
        }
	}


    public void BetterExplode(PlayerControl source, float radius)
    {
        Vector2 dir;
        for (int i = 1; i < 9; i++)
        {
            dir = Quaternion.AngleAxis(i*45f,Vector3.forward)*Vector2.up;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, radius, LayerMask.GetMask(new string[] { "Enemy", "Player", "Ground", "BackgroundBlock" }));
            if (hit)
            {
                if (hit.rigidbody)
                    //Debug.Log(hit.transform.gameObject);
                    hit.rigidbody.AddForce(dir*1000);
                // If the hit object is damageable...
                if (hit.collider.gameObject.GetComponent<Damageable>() != null)
                {
                    // ...give it 10 hitpointd of damage
                    hit.collider.gameObject.GetComponent<Damageable>().TakeDamage(5);
                }

                // Otherwise if the player manages to shoot himself...
                else if (hit.collider.gameObject.tag == "Player")
                {

                    // Instantiate the explosion and destroy the rocket.
                    PlayerHealth pH = hit.collider.gameObject.GetComponent<PlayerHealth>();
                    pH.TakeDamage(gameObject.transform, source);
                    //OnExplode();
                }
            }
        }
    }

}
