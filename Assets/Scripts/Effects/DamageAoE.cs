using UnityEngine;
using System.Collections;

public class DamageAoE : Damage {

	public float Radius = 10;
	public LayerMask InteractWith;

	protected override void Execute (){

		/*
		GameObject[] newTargets = GetTargetsInArea();
		if(newTargets == null)
			return;

		if(_targets == null)
			_targets = newTargets;
		else{
			GameObject[] merge = new GameObject[newTargets.Length+_targets.Length];
			_targets.CopyTo(merge,0);
			newTargets.CopyTo(merge, _targets.Length);
			_targets = merge;
		}*/

		_targets = GetTargetsInArea();

		if(_targets == null)
			return;

		base.Execute();
	}

	protected virtual GameObject[] GetTargetsInArea(){
		Collider2D[] coll = Physics2D.OverlapCircleAll(transform.position, Radius, InteractWith);

		if(coll == null)
			return null;

		GameObject[] newTargets = new GameObject[coll.Length];
		for(int i=0;i<coll.Length;i++){
			newTargets[i] = coll[i].gameObject;
		}
		return newTargets;

	}
}
