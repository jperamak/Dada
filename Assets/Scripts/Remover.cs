using UnityEngine;
using System.Collections;

public class Remover : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D col)
	{
		// If the player hits the trigger...

			Destroy (col.gameObject);	

	}
}
