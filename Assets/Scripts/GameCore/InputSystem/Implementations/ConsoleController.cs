using UnityEngine;
using System.Collections.Generic;

namespace Dada.InputSystem{
public class ConsoleController : AbstractController {

		private string suffix;
		private Dictionary<VirtualKey, float> _btnOccurrence = new Dictionary<VirtualKey, float>();

		public override float XAxis{get{ return GetAxis(VirtualKey.X_AXIS);}}
		public override float YAxis{get{ return GetAxis(VirtualKey.Y_AXIS);}}
		public override bool AnyKey{get{return UnityEngine.Input.anyKey;}}

		public ConsoleController(KeyMap keymap,string name) : this(keymap,name,0){}
		public ConsoleController(KeyMap keymap,string name, int number) : base(keymap,name,number){
			suffix = "J"+number+" ";
		}


		public override float GetAxis(VirtualKey key){

			float val = ReadFromJoystick(key);
			if(Inverted)
				return -val;
			return val;
		}

		public override bool GetButton(VirtualKey key){
			float val = ReadFromJoystick(key);

			if(val == 0)
				return false;

			_btnOccurrence[key] = Time.time;
			return true;
		}

		public override bool GetButtonDown(VirtualKey key){

			//BUG!! The last occurrence is registered correctly only if GetButtonDown is called af every frame.
			//For other cases it may fail hard....!

			/*
			//get the last occurrence of this key
			float lastOccurrence = 0;
			if(_btnOccurrence.ContainsKey(key))
				lastOccurrence = _btnOccurrence[key];

			//the button is pressed in this frame
			if(GetButton(key)){
				float diff = Mathf.Abs( (_btnOccurrence[key] - lastOccurrence) - Time.deltaTime);

				//the button was pressed also in the previous frame
				if(diff < 0.00001f)
					return false;
				else 
					return true;
			}
			return false;
*/
			//this version works fine for normal buttons
			List<KeyProperty> keys = _keymap.Get(key);
			if(keys == null)
				return false;

			foreach(KeyProperty prop in keys)
				if(UnityEngine.Input.GetButtonDown(suffix+prop.Name))
					return true;
			return false;
		}

		public override bool GetButtonUp(VirtualKey key){

			/*
			//button being pressed
			if(GetButton(key))
				return false;

			//button never pressed
			if(!_btnOccurrence.ContainsKey(key))
				return false;

			float diff = Mathf.Abs( (Time.time - _btnOccurrence[key]) - Time.deltaTime);

			//the button was pressed in the previous frame but now is not
			if(diff < 0.00001f)
				return true;

			return false;
*/

			//this version works fine for normal buttons
			List<KeyProperty> keys = _keymap.Get(key);
			if(keys == null)
				return false;
			
			foreach(KeyProperty prop in keys)
				if(UnityEngine.Input.GetButtonUp(suffix+prop.Name))
					return true;
			return false;
		}

		private float ReadFromJoystick(VirtualKey key){
			List<KeyProperty> keys = _keymap.Get(key);

			if(keys == null || keys.Count == 0)
				return 0;
			
			float sum = 0;
			float val;
			foreach(KeyProperty prop in keys){
				
				//read value from device
				if(prop.IsAxis)
					val = UnityEngine.Input.GetAxis(suffix+prop.Name);
				else
					val = UnityEngine.Input.GetButton(suffix+prop.Name) ? 1 : 0;
				
				//Invert if necessary
				if(prop.Inverted)
					val = -val;

				//Debug.Log(Name+ " read " + prop.Name +" "+val);

				if(prop.TriggerCondition == KeyTriggerCondition.NON_ZERO)
					sum += val;
				else if(prop.TriggerCondition == KeyTriggerCondition.POSITIVE && val > 0)
					sum += val;
				else if(prop.TriggerCondition == KeyTriggerCondition.NEGATIVE && val < 0)
					sum += val;
				else if(prop.TriggerCondition == KeyTriggerCondition.ABS_ONE && Mathf.Abs(val) == 1)
					sum += val;
				else if(prop.TriggerCondition == KeyTriggerCondition.ONE && val == 1)
					sum += 1;
				else if(prop.TriggerCondition == KeyTriggerCondition.MINUS_ONE && val == -1)
					sum -= 1;
			}

			//avoid false trigger due to hardware imprecisions
			if( Mathf.Abs(sum) < 0.01f)
				return 0;

			return Mathf.Clamp(sum,-1,1);
		}
}
}