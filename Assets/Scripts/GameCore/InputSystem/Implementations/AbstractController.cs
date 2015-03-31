using UnityEngine;
using System.Collections.Generic;

namespace Dada.InputSystem{
	
public abstract class AbstractController {

		protected KeyMap _keymap;

		public string Name{get; private set;}
		public int Number{get; private set;}

		public abstract float XAxis{get;}
		public abstract float YAxis{get;}
		public abstract bool AnyKey{get;}

		public virtual bool Inverted{get; set;}
		public virtual float Sensibility{get;set;}
		public virtual float DeadZone{get;set;} 

		public abstract float GetAxis(VirtualKey key);
		public abstract bool GetButton(VirtualKey key);
		public abstract bool GetButtonDown(VirtualKey key);
		public abstract bool GetButtonUp(VirtualKey key);



		public AbstractController(KeyMap keymap, string name, int number){

			//Debug.Log("Created joystick "+number+": "+name);

			Name = name;
			Number = number;
			Inverted = false;
			Sensibility = 1;
			//DeadZone = 0.25f;
			_keymap = new KeyMap();

			if(keymap != null)
				_keymap = keymap;
		}

}
}