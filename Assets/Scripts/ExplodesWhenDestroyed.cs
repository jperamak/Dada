using UnityEngine;
using System.Collections;

public class ExplodesWhenDestroyed : MonoBehaviour {
		
		public GameObject Explosion;		// Prefab of explosion effect.
		public float Radius;
		public float ExplosionForce;
		public int Damage;

	bool isAppQuittingDown = false;

		
		void OnDestroy () {

			if (isAppQuittingDown) // New objects should not be created when app is quitting as they are not cleaned then
				return;

			Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
			// Instantiate the explosion with random rotation.
			GameObject expl = (GameObject)Instantiate(Explosion, transform.position, randomRotation);
			expl.GetComponent<Explosion>().Explode(null, Radius, ExplosionForce, Damage);
			Destroy(gameObject);
		}

		void OnApplicationQuit() {
			isAppQuittingDown = true;
		}

	}
