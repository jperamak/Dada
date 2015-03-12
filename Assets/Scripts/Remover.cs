using UnityEngine;
using System.Collections;

public class Remover : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        
		if (col == null)
			return;

		// If the player hits the trigger...
        if (col.gameObject.tag == "Player")
        {
			HeroHealth pc = col.gameObject.GetComponent<HeroHealth>();
            pc.Kill();
        }
        else
        {
            Destroy(col.gameObject);
        }
    }

	void OnCollisionEnter2D(Collision2D col){
		// If the player hits the trigger...
		if (col == null)
			return;
		if(col.gameObject.tag == "Player")
		{
			HeroHealth pc = col.gameObject.GetComponent<HeroHealth>();
			pc.Kill();
		}
		else
		{
			Destroy(col.gameObject);
		}
	}




}
