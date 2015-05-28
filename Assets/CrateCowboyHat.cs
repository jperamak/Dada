using UnityEngine;
using System.Collections;

public class CrateCowboyHat : MonoBehaviour {

	public Transform target;
	public float floatSpeed;

	float awakeTime;

	// Use this for initialization
	void Awake () {
		awakeTime = Time.time;

	}
	
	// Update is called once per frame
	void Update () {

		//if (target != null) {
//			Vector3 targetVec = target.transform.position - transform.position;

			transform.position = Vector2.MoveTowards(transform.position, target.transform.position, floatSpeed*Time.deltaTime);
		//}


		if (Time.time - awakeTime > 0.5f && (target.transform.position - transform.position).magnitude < 0.1f) {
			Animation hatAnim = this.GetComponent<Animation>();
			if (hatAnim != null)
				hatAnim.enabled = false;
			transform.eulerAngles = new Vector3(0f,0f,0f);
			transform.position = target.transform.position;
		}




	}
}
