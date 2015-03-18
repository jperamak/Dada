using UnityEngine;
using System.Collections;

public class RegenerateAfterDestroyed : MonoBehaviour {

	public float regenerateDelay;
	private Vector3 _originalScale;

	private Damageable _damageable;

	void Start () {
		_originalScale = transform.localScale;
		_damageable = gameObject.GetComponent<Damageable>();

		_damageable.OnDestroy += OnZeroHp;
	}
	

	void Regenerate() {

		gameObject.SetActive(true);
		if (_damageable != null) 
			_damageable.RestoreToMaxHp();
			
		StartCoroutine("Appear");
	}


	// Makes the object appear by increasing its scale
	IEnumerator Appear() {
		for (float scale = 0f; scale < 1f; scale += 0.03f) {
			transform.localScale = _originalScale * scale;
			yield return null;
		}
		transform.localScale = _originalScale;
	}


	void OnZeroHp(GameObject notneeded, GameObject notneeded2) {
		Invoke ("Regenerate", regenerateDelay );
		gameObject.SetActive(false);

	}
}
