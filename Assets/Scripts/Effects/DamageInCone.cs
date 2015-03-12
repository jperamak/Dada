using UnityEngine;
using System.Collections.Generic;

public class DamageInCone : DamageAoE {
	
	public float Angle;

	//BUG! This should use raycast instead or OnCollisionEnter to detect the exact contact point. Need optimization
	

	protected override GameObject[] GetTargetsInArea(){

		//get all components in the attack radius
		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position,Radius,InteractWith);
		List<GameObject> targets = new List<GameObject>();

		for(int i=0; i< colliders.Length; i++){
			Collider2D coll = colliders[i];
			
			//is target inside the cone?
			if(isInRange(coll.bounds.center, transform.right))
				targets.Add(coll.gameObject);
		}

		return targets.ToArray();
	}

	//check if the given position is inside the cone facing the attacking direction
	private bool isInRange(Vector2 targetPos, Vector2 attackDir){
		Vector2 dirToTarget = (Vector2)transform.position - targetPos;
		float angle = Vector2.Angle(attackDir, dirToTarget);
		
		if(angle < Angle/2)
			return true;
		return false;
	}
}
