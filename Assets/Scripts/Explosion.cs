using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

    public void Explode(PlayerControl source, float radius, float explosionForce, int damage)
    {
        Vector2 dir;
        Vector2 position = new Vector2(transform.position.x, transform.position.y);

        //float divider = 4 * radius > 4 ? 4 * radius : 4;
		float divider = 64f; 
		float invDiv = 1/divider;
        float step = 360f * invDiv;

        for (int i = 0; i < divider; i++)
        {
            dir = Quaternion.AngleAxis(step*0.5f + step*i,Vector3.forward)*Vector2.up;
            RaycastHit2D[] hits = Physics2D.LinecastAll(position, position + dir * radius, LayerMask.GetMask(new string[] { "Enemy", "Player", "Ground", "BackgroundBlock", "Rubble" }));
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.rigidbody)
                    hit.rigidbody.AddForce(dir * 10 * explosionForce);

                // If the hit object is damageable...
                if (hit.collider.gameObject.GetComponent<Damageable>() != null)
                {
                    // ...give it the damage
                    hit.collider.gameObject.GetComponent<Damageable>().TakeDamage((float)damage * invDiv);
                }

                // Otherwise if the player manages to shoot himself...
                else if (hit.collider.gameObject.tag == "Player")
                {

                    // Instantiate the explosion and destroy the rocket.
                    PlayerHealth pH = hit.collider.gameObject.GetComponent<PlayerHealth>();
                    pH.TakeDamage(gameObject.transform, source);
                }
                // dont hit things behind solid blocks
                if (hit.transform.gameObject.layer == LayerMask.GetMask("Ground"))
                    break;
            }
        }
    }

}
