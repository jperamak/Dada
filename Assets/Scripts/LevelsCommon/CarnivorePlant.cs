using UnityEngine;
using System.Collections;


// I was drunk as shit when I coded this so be bear with with me.
public class CarnivorePlant : MonoBehaviour {

	public float detectHeroRange;
	public float force;
	Transform forcePoint;

	// Use this for initialization
	void Start () {
		forcePoint = transform.FindChild("ForcePosition");
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 preyVector = GetVectorToClosestHero();

		if (preyVector.magnitude < detectHeroRange) {
			preyVector.Normalize();
			preyVector = Vector2.Scale( preyVector, new Vector2(force,force));
			this.GetComponent<Rigidbody2D>().AddForceAtPosition(preyVector, forcePoint.position, ForceMode2D.Force);
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
