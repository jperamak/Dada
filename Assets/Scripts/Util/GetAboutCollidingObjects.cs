using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// About because we only test collision with bounding box not the actual colliders
public class GetAboutCollidingObjects {

	public static GameObject[] With(GameObject obj){

		Collider2D[] colliders = obj.GetComponents<Collider2D>();

		List<GameObject> results = new List<GameObject>();

		foreach (Collider2D col in colliders) {

			Collider2D[] otherColliders = Physics2D.OverlapAreaAll( col.bounds.min, col.bounds.max );
				foreach (Collider2D otherCol in otherColliders) {

					if (!results.Contains(otherCol.gameObject) && obj!=otherCol.gameObject) {
						results.Add(otherCol.gameObject);
				}
				// Only works after physics system update!!!
				//if (otherCol.IsTouching(col)) {
				//		Debug.Log(otherCol.gameObject.name);
						
				//	}
				}

			}

		return results.ToArray();
	}



}
