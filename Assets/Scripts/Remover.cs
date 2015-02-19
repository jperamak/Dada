using UnityEngine;
using System.Collections;

public class Remover : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D col)
	{
		// If the player hits the trigger...
		if(col.gameObject.tag == "Player")
		{
            PlayerHealth ph = col.gameObject.GetComponent<PlayerHealth>();
            ph.health = 0;
            ph.TakeDamage(transform, col.gameObject.GetComponent<PlayerControl>());
		}
		else
		{
			Destroy (col.gameObject);	
		}
	}
}
