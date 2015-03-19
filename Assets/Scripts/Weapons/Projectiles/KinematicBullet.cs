using UnityEngine;
using System.Collections;

public class KinematicBullet : SimpleBullet {

	public float Speed = 10.0f;
	
	void Update () {
		transform.position = transform.position + (transform.right * Speed * Time.deltaTime);
	}
}
