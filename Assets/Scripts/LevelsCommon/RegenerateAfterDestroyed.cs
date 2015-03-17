using UnityEngine;
using System.Collections;

public class RegenerateAfterDestroyed : MonoBehaviour {

	public float regenerateDelay;
	private Vector3 _originalScale;

	void Start () {
		_originalScale = transform.localScale;
	}
	

	void Regenerate() {
		Damageable dam = gameObject.GetComponent<Damageable>();
		gameObject.SetActive(true);
		if (dam != null) 
			dam.RestoreToMaxHp();
			
		StartCoroutine("FadeIn");
	}


	IEnumerator FadeIn() {
		for (float scale = 0f; scale < 1f; scale += 0.03f) {
			transform.localScale = _originalScale * scale;
			yield return null;
		}
		transform.localScale = _originalScale;
	}

	void OnZeroHp() {
		Invoke ("Regenerate", regenerateDelay );
		gameObject.SetActive(false);

	}
}
