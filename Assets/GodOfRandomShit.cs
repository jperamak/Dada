using UnityEngine;
using System.Collections;

public class GodOfRandomShit : MonoBehaviour {

	public float speedPerSec = 0.0f;
	public float verticalMovement = 0.0f;


	float altitude;

	// Use this for initialization
	void Start () {
		altitude = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(transform.position.x + speedPerSec * Time.deltaTime, 
		                                  altitude + Mathf.Sin(Time.time * 2f ) * verticalMovement , 0f);
	}
}