using UnityEngine;
using System.Collections;


// I was drunk as shit when I coded this so be bear with with me.
public class CarnivorePlant : MonoBehaviour {

	public float detectRange;
	public float attackRange;
	public float detectPullForce;
	public float attackPullForce;


	Transform forceMarker;

	float nextMunch = 0f;

	// Use this for initialization
	void Start () {
		forceMarker = transform.FindChild("ForceMarker");
		if (forceMarker == null)
			Debug.LogError("No forcemarker found for carnivore plant");
	}


	// Update is called once per frame
	void Update () {

	
		Vector2 heroVector = GetVectorToClosestHero();

		if (heroVector.magnitude < detectRange) {

			Vector2 forceVector = Vector2.Scale( heroVector.normalized, new Vector2(detectPullForce, detectPullForce));

			// force is stronger if player is very close
			if (heroVector.magnitude < attackRange)
				forceVector = Vector2.Scale( heroVector.normalized, new Vector2(attackPullForce, attackPullForce));

			// apply force to direction of player
			//this.GetComponent<Rigidbody2D>().AddForce(forceVector,ForceMode2D.Force);
			this.GetComponent<Rigidbody2D>().AddForceAtPosition(forceVector, forceMarker.position, ForceMode2D.Force);

		}

		if (heroVector.magnitude < attackRange) {
	
			// start to open and close mouth
			if (Time.time > nextMunch) {
				if (this.GetComponent<SpringJoint2D>().distance > 0.5f) {
					this.GetComponent<SpringJoint2D>().distance = 0.1f;
					nextMunch = Time.time + 0.4f;
				}
				else {
					this.GetComponent<SpringJoint2D>().distance = 1.9f;
					nextMunch = Time.time + 0.4f;
				}
			}
		}


	}



	Vector2 GetVectorToClosestHero() {

		// TODO: Ineffective, fix
		Object[] heros = FindObjectsOfType(typeof(Hero));
		
		Hero closestHero = null;
		
		for (int i=0; i<heros.Length; i++) {
			Hero hero = (Hero)heros[i];

			if (closestHero == null)
				closestHero = hero;
			else if (RangeToHero(hero) < RangeToHero(closestHero))
				closestHero = hero;
		}

		if (closestHero == null)
			return new Vector2(0f,0f);

		return closestHero.transform.position - transform.position;

	}

	float RangeToHero(Hero hero) {
		return Vector2.Distance( hero.transform.position, transform.position );
	}

}
