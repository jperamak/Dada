using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GravityLevelManager : LevelManager {

	public override void InitLevel (){

		PointEffector2D[] moons = GameObject.FindObjectsOfType<PointEffector2D>();
		for(int i=0; i<moons.Length; i++)
			moons[i].gameObject.AddComponent<GravityArea>();

		base.InitLevel ();
	}

	public override GameObject SpawnHero (Player p, Vector2 newSpawnPoint){

		GameObject hero = base.SpawnHero (p, newSpawnPoint);
		HeroControllerV2 hc2 = hero.GetComponent<HeroControllerV2>();
		if(hc2 != null){
			Destroy(hc2);
			hero.AddComponent<HeroController>();
		}

		return hero;
	}
}
