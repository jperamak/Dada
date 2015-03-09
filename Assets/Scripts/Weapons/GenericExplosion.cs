using UnityEngine;
using System.Collections;

public class GenericExplosion : MonoBehaviour {

	public GameObject Explosion;		// Prefab of explosion effect.
	public float Radius;
	public float ExplosionForce;
	public int Damage;
	public PlayerControl Player;
	
	void Awake () {
		Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
		
		// Instantiate the explosion with random rotation.
		GameObject expl = (GameObject)Instantiate(Explosion, transform.position, randomRotation);
		expl.GetComponent<Explosion>().Explode(Player, Radius, ExplosionForce, Damage);
		Destroy(gameObject);
	}
}
