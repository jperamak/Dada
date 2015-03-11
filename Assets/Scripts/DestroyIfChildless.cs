using UnityEngine;
using System.Collections;

public class DestroyIfChildless : MonoBehaviour {

	// could also be checked less often...
	void Update () 
	{
		if (transform.childCount == 0)
			Destroy(gameObject);
	}
}
