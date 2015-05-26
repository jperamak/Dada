using UnityEngine;
using System.Collections;

public class RandomVelocityAtBirth : MonoBehaviour {

	public float minVelocity = 0f;
	public float maxVelocity = 1f;

	// Use this for initialization
	void Start () {
		Rigidbody2D  rb = gameObject.GetComponent<Rigidbody2D>();
		if (rb == null) {
			Debug.LogError("RandomVelocitAtBirth attached to object without RigidBody2D");
			return;
		}

		Vector3 direction = ZRotateVector(Vector3.right, Random.Range(0f,360f));
		float speed = Random.Range(minVelocity, maxVelocity);
		rb.velocity += new Vector2( direction.x * speed, direction.y * speed );

	
	}

	private Vector3 ZRotateVector(Vector3 vec, float angle )
	{
		Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
		return q * vec;
	}

	

}