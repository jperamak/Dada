using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

    public void Explode(PlayerControl source, float radius, float explosionForce, int damage)
    {
        Vector2 dir;
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        for (int i = 0; i < 16; i++)
        {
            dir = Quaternion.AngleAxis(11.75f+22.5f*i,Vector3.forward)*Vector2.up;
            RaycastHit2D[] hits = Physics2D.LinecastAll(position, position + dir * radius, LayerMask.GetMask(new string[] { "Enemy", "Player", "Ground", "BackgroundBlock", "Rubble" }));
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.rigidbody)
                    hit.rigidbody.AddForce(dir * 10 * explosionForce);

                // If the hit object is damageable...
                if (hit.collider.gameObject.GetComponent<Damageable>() != null)
                {
                    // ...give it 10 hitpointd of damage
                    hit.collider.gameObject.GetComponent<Damageable>().TakeDamage(damage);
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
