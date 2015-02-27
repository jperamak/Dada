using UnityEngine;
using System.Collections;

public class MeleeStrike : MonoBehaviour {

	public float damage;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D (Collider2D col) 
	{
		
		// If the hit object is damageable...
		if (col.gameObject.GetComponent<Damageable>() != null) {
			// ...give it 10 hitpointd of damage
			col.gameObject.GetComponent<Damageable>().TakeDamage(damage);
		}
		
		// BUG: Does damage to self also
		if(col.gameObject.tag == "Player")
		{
			PlayerHealth pH = col.gameObject.GetComponent<PlayerHealth>();
			pH.TakeDamage(gameObject.transform, null);
		}
	}
}
