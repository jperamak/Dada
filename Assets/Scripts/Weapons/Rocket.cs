using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour 
{
	public GameObject explosion;		// Prefab of explosion effect.
    public float radius;
    [HideInInspector]
    public PlayerControl player;

	void Start () 
	{
		// Destroy the rocket after 4 seconds if it doesn't get destroyed before then.
        Invoke("OnExplode", 4);
        //Destroy(gameObject, 4);
	}
	
	void Update ()
	{
		Vector2 velocity = gameObject.GetComponent<Rigidbody2D>().velocity;
		float angle = Mathf.Atan2( velocity.y, velocity.x );
		transform.eulerAngles = new Vector3(0, 0, angle *  Mathf.Rad2Deg);
	}
	
	void OnExplode()
	{
		// Create a quaternion with a random rotation in the z-axis.
		Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
		
		// Instantiate the explosion where the rocket is with the random rotation.
        GameObject expl = (GameObject)Instantiate(explosion, transform.position, randomRotation);
        expl.GetComponent<Explosion>().BetterExplode(player, radius);
        Destroy(gameObject);
	}
	
	void OnTriggerEnter2D (Collider2D c) 
	{
        OnExplode();
    }
	
}
