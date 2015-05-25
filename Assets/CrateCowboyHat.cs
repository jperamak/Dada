using UnityEngine;
using System.Collections;

public class CrateCowboyHat : MonoBehaviour {

	public Transform target;
	public float floatSpeed;

	float awakeTime;
	Animation hatAnim;
	bool hatInHead = false;

	// Use this for initialization
	void Awake () {
		awakeTime = Time.time;
		hatAnim = this.GetComponent<Animation>();
	}
	
	// Update is called once per frame
	void Update () {

		//if (target != null) {
//			Vector3 targetVec = target.transform.position - transform.position;

			transform.position = Vector2.MoveTowards(transform.position, target.transform.position, floatSpeed*Time.deltaTime);
		//}

		if (awakeTime > 0.5f && (target.transform.position - transform.position).magnitude < 0.1f) {
			hatInHead = true;
		}




	}
}
