using UnityEngine;
using System.Collections;

namespace Dada.InputSystem{

public class NullController : AbstractController {

		public override float XAxis{get{ return 0;}}
		public override float YAxis{get{ return 0;}}
		public override bool AnyKey{get{return false;}}
		
		public NullController(int number) : base(null,"NullController",number){}
		
		public override float GetAxis(VirtualKey key){return 0;}
		public override bool GetButton(VirtualKey key){return false;}
		public override bool GetButtonUp(VirtualKey key){return false;}
		public override bool GetButtonDown(VirtualKey key){return false;}
}
}