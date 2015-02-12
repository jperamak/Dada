using UnityEngine;
using Dada.InputSystem;
using System.Collections.Generic;

public class InputInitialize : MonoBehaviour {

	// Use this for initialization
	void Awake () {

		TextAsset txt = (TextAsset)Resources.Load("keymap", typeof(TextAsset));
		string json = txt.text;

		Dictionary<string,KeyMap> keyMapConfig = KeyMap.JsonToKeyConfiguration(json);
		
		#if UNITY_EDITOR
		DadaInput.Initialize(keyMapConfig);
		#else
		DadaInputInput.Initialize(keyMapConfig,InputMethod.JOYSTICK);
		#endif

	}
}
