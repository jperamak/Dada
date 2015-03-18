using UnityEngine;
using System.Collections;

public class CreatesWhenDestroyed : MonoBehaviour {


	public GameObject[] gameObjects;

	void OnZeroHp() {

		foreach (GameObject g in gameObjects) {
			GameObject ruin = Instantiate(g, transform.position, transform.rotation) as GameObject;
			ruin.transform.localScale = transform.localScale;
		}

	}

}
