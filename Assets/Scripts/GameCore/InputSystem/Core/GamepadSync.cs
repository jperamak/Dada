using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// At the end of the frame the gamepad controllers need to clear the information about the button pressed.
/// This is necessary for supporting GetButtonDown and GetButtonUp emulation for axis
/// </summary>

namespace Dada.InputSystem{
	public class GamepadSync : MonoBehaviour {

		private static List<ConsoleController> _controllers;
		private static GamepadSync Instance;

		//Auto-instantiate the gameobject and set it permanent
		public static void Initialize(List<ConsoleController> c){
			if(Instance != null)
				return;

			GameObject go = new GameObject("GamepadSync");
			Instance = go.AddComponent<GamepadSync>();
			_controllers = c;
			DontDestroyOnLoad(Instance);
		}

		void LateUpdate () {
			if(_controllers != null)
				for(int i=0;i<_controllers.Count;i++)
					_controllers[i].RefreshFrameInfo();
		}
	}
}