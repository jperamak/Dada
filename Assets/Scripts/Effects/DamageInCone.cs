using UnityEngine;
using System.Collections.Generic;

public class DamageInCone : DamageAoE {
	
	public float Angle;
	private int _numRays = 16;


	protected override GameObject[] GetTargetsInArea(){

		float step = Angle / _numRays;
		float ang;
		Vector3 dir;
		RaycastHit2D[] hit;
		HashSet<GameObject> targets = new HashSet<GameObject>();

		for(int i=0;i < _numRays; i++){
			ang = (step*i)-(step*_numRays/2); 
			dir = Quaternion.AngleAxis(ang,Vector3.forward)*transform.right;
			hit = Physics2D.RaycastAll(transform.position,dir,Radius,InteractWith);

			//multiple rays may hit the same object several times. A hashset ensures that only one copy of every target
			//is included (and damaged)
			for(int j=0;j<hit.Length;j++)
				targets.Add(hit[j].collider.gameObject);

			Debug.DrawLine(transform.position, transform.position+(dir*Radius) , Color.red, 1); 

		}
		GameObject[] _targetArray = new GameObject[targets.Count];
		targets.CopyTo(_targetArray);
		return _targetArray;
	}
}
