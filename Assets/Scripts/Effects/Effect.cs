using UnityEngine;
using System.Collections;

public abstract class Effect : MonoBehaviour {

	public GameObject VisualEffect;

	public Callback OnEnd;
	public GameObject Owner{get;set;}
	protected GameObject[] _targets;

	public virtual void Trigger(){
		Execute();
		DoVisualEffect();
		Terminate();
	}

	public virtual void Trigger(GameObject target){
		_targets = new GameObject[]{target};
		Trigger();
	}
	public virtual void Trigger(GameObject[] targets){
		_targets = targets;
		Trigger();
	}

	public virtual void Terminate(float delay = 0){
		if(OnEnd != null)
			OnEnd();
		Destroy(this, delay);
	}

	protected virtual  void DoVisualEffect(){
		if(VisualEffect != null)
			Instantiate(VisualEffect,transform.position, transform.rotation);
	}

	protected abstract void Execute();
}
