using UnityEngine;
using System.Collections;

public class DestroyIfDestroyed : MonoBehaviour {

	public GameObject target;

	void OnDestroy () {
		if (target!=null)
			Destroy(target);
	}
}
