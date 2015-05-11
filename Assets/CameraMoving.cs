using UnityEngine;
using System.Collections;

public class CameraMoving : MonoBehaviour {

	public float ySpeed = 0;
	public float xSpeed = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += new Vector3(Time.deltaTime * xSpeed, Time.deltaTime * ySpeed, 0f);
	}
}
