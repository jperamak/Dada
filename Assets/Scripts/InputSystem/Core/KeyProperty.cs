using UnityEngine;
using System.Collections.Generic;
using System;

namespace Dada.InputSystem{

	public enum KeyTriggerCondition{ POSITIVE, NEGATIVE, ONE, MINUS_ONE, ABS_ONE, NON_ZERO };
	public enum AxisOrientation{ VERTICAL, HORIZONTAL, NONE };
public class KeyProperty : ICloneable{



		public string Name {get; private set;}
		public bool IsAxis{get; private set;}
		public AxisOrientation Orientation{get; private set;}
		public bool Inverted{get; private set;}
		public KeyTriggerCondition TriggerCondition{get; private set;}
		public KeyCode ToKeycode {get; private set;}

		public KeyProperty(string name, bool isAxis = false, bool inverted = false, KeyTriggerCondition condition = KeyTriggerCondition.NON_ZERO, AxisOrientation orientation = AxisOrientation.NONE ){
			Name = name;
			IsAxis = isAxis;
			Inverted = inverted;
			TriggerCondition = condition;
			Orientation = orientation;

			try {
				ToKeycode = (KeyCode) Enum.Parse(typeof(KeyCode), name);
			}
			catch (ArgumentException) {
				ToKeycode = KeyCode.None;
			}
		}


		public object Clone(){
			return new KeyProperty(Name, IsAxis, Inverted, TriggerCondition);
		}

	
}
}