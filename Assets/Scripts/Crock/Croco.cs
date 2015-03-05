using UnityEngine;
using System.Collections;

public class Croco : MonoBehaviour {

	private Transform _head;

	void Start(){

		_head = transform.FindChild("Head");
		int childrenNum = transform.childCount;

		for(int i=0;i<childrenNum;i++){
			Damageable dmg = transform.GetChild(i).gameObject.AddComponent<Damageable>();
			dmg.maxHitpoints = 100.0f;
			dmg.currentHitpoints = 100.0f;
		}

	}
}
