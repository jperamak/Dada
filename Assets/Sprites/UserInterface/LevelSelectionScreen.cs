using UnityEngine;
using System.Collections;

public class LevelSelectionScreen : MonoBehaviour {

	public void LoadLevel(string level){
		Application.LoadLevel(level);
	}
}
