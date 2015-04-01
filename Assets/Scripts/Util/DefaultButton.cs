using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DefaultButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Button b = GetComponent<Button>();
		if(b != null)
			b.Select();
	}
}
