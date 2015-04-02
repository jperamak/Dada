using UnityEngine;
using System.Collections.Generic;

public class GravityArea : MonoBehaviour {

	private List<Hero> _heroesInside = new List<Hero>();
	private static Dictionary<Collider2D,Pair<float, int>> _affectedObjects = new Dictionary<Collider2D, Pair<float, int>>();

	void OnTriggerEnter2D(Collider2D coll){
		Hero hero = coll.GetComponent<Hero>();

		//Triggers shall not pass!
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

				//hero has not the ability to walk on the planet
				if(hero.GetComponent<HeroPlanetController>() == null){

					//disable existing script controller and add the custom one
					hero.GetComponent<HeroController>().enabled = false;
					hero.gameObject.AddComponent<HeroPlanetController>();
				}
			}
		}

		//all other objects are saved in a dictionary to handle their gravity when they enter inside an effector area
		else{
			//for objects entered for the first time 
			if(!_affectedObjects.ContainsKey(coll)){
				//create an entry in the dictionary and save its original gravity scale
				_affectedObjects.Add(coll,new Pair<float, int>(coll.attachedRigidbody.gravityScale,1));
				//then set the gravity scale to 0.
				coll.attachedRigidbody.gravityScale = 0;
			}
			//for objects who are already under other effector areas, just increase the counter
			else
				_affectedObjects[coll].Second += 1; 


		}
	}

	void OnTriggerExit2D(Collider2D coll){
		Hero hero = coll.GetComponent<Hero>();

		//Triggers shall not pass!
		if(coll.isTrigger)
			return;

		//only heroes must have the aligner script
		if(hero != null){
			
			//prevent from running the same code two times (because the hero have 2 colliders)
			if(_heroesInside.Contains(hero)){
				_heroesInside.Remove(hero);
				AlignWithAttractorPoint alignScript = coll.GetComponent<AlignWithAttractorPoint>();
				if(alignScript != null){

					//hero is about to exit the last area
					if(alignScript.Count == 1){
						
						//disable existing script controller and add the custom one
						Destroy(hero.GetComponent<HeroPlanetController>());
						hero.GetComponent<HeroController>().enabled = true;
					}
					alignScript.RemovePoint(transform);
				}
			}
		}

		//if an object was under effect of an area effector
		else if(_affectedObjects.ContainsKey(coll)){
			//decrease the area counter
			_affectedObjects[coll].Second -= 1;
			//the object is no more influenced by gravity areas
			if(_affectedObjects[coll].Second == 0){
				//set its original gravity scale and remove it from the list
				coll.attachedRigidbody.gravityScale = _affectedObjects[coll].First;
				_affectedObjects.Remove(coll);
			}
		}
	}
}
