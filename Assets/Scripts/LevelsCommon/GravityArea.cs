using UnityEngine;
using System.Collections.Generic;

public class GravityArea : MonoBehaviour {

	private List<Hero> _heroesInside = new List<Hero>();

	void OnTriggerEnter2D(Collider2D coll){
		Hero hero = coll.GetComponent<Hero>();

		//no triggers allowed!
		if(coll.isTrigger)
			return;

		//only heroes must have the aligner script
		if(hero != null){

			//prevent from running the same code two times (because the hero have 2 colliders)
			if(!_heroesInside.Contains(hero)){
				_heroesInside.Add(hero);
				AlignWithAttractorPoint alignScript = coll.GetComponent<AlignWithAttractorPoint>();
				if(alignScript == null)
					alignScript = coll.gameObject.AddComponent<AlignWithAttractorPoint>();
				alignScript.AddPoint(transform);
			}
		}
		else{
			//Debug.Log("Enter in "+name+" "+coll.name);
			coll.attachedRigidbody.gravityScale = 0;
		}
	}

	void OnTriggerExit2D(Collider2D coll){
		Hero hero = coll.GetComponent<Hero>();

		//no triggers allowed!
		if(coll.isTrigger)
			return;

		//only heroes must have the aligner script
		if(hero != null){
			
			//prevent from running the same code two times (because the hero have 2 colliders)
			if(_heroesInside.Contains(hero)){
				_heroesInside.Remove(hero);
				AlignWithAttractorPoint alignScript = coll.GetComponent<AlignWithAttractorPoint>();
				if(alignScript != null)
					alignScript.RemovePoint(transform);
			}
		}
		else{
			//Debug.Log("Exit from "+name+" "+coll.name);
			coll.attachedRigidbody.gravityScale = 1;
		}
	}
}
