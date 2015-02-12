using UnityEngine;
using System.Collections.Generic;

namespace Dada.InputSystem{
public class CompositeController : AbstractController {

		private List<AbstractController> _joyList;

		public override float XAxis{get{ return getXAxis();}}
		public override float YAxis{get{ return getYAxis();}}

		public override bool AnyKey{get{return UnityEngine.Input.anyKey;}}

		public override float GetAxis(VirtualKey key){
			float sum = 0;
			foreach(AbstractController joy in _joyList)
				sum += joy.GetAxis(key);
			
			return Mathf.Clamp(sum,-1,1);
		}

		public override bool GetButton(VirtualKey key){
			foreach(AbstractController joy in _joyList)
				if(joy.GetButton(key))
					return true;
			return false;
		}
		
		public override bool GetButtonDown(VirtualKey key){
			foreach(AbstractController joy in _joyList)
				if(joy.GetButtonDown(key))
					return true;
			return false;
		}
		public override bool GetButtonUp(VirtualKey key){
			foreach(AbstractController joy in _joyList)
				if(joy.GetButtonUp(key))
					return true;
			return false;
		}
		
		
		public CompositeController(List<AbstractController> list) : base(null,"Composite Joystick",-1){
			_joyList = list;
		}
		
		
		private float getXAxis(){
			float sum = 0;
			foreach(AbstractController joy in _joyList)
				sum += joy.XAxis;
			
			return Mathf.Clamp(sum,-1,1);
		}

		private float getYAxis(){
			float sum = 0;
			foreach(AbstractController joy in _joyList)
				sum += joy.YAxis;
			
			return Mathf.Clamp(sum,-1,1);
		}
}
}