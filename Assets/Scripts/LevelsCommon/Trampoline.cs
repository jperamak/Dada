using UnityEngine;
using System.Collections;

public class Trampoline : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col){
		Hero hero = col.gameObject.GetComponent<Hero>();
		
		if (hero == null )
			return;

		Rigidbody2D rb = hero.GetComponent<Rigidbody2D>();
		rb.velocity = new Vector3(0f, 80f,0f); 
		

	}
}
