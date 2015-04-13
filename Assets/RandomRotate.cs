using UnityEngine;
using System.Collections;

public class RandomRotate : MonoBehaviour {

	// Use this for initialization
	void Start () {
        transform.rotation = transform.rotation * Quaternion.Euler(new Vector3(0, 0, 90 * Random.RandomRange(0, 3)));
        Debug.Log(transform.rotation);
	}

}
