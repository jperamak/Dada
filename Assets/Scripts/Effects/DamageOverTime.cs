using UnityEngine;
using System.Collections;

public class DamageOverTime : DamageAoE {

	public float DamageInterval = 0.5f;
	public float DurationTime = 3.0f;

	private bool _executing = false;
	private float _nextDamage;
	private float _endTime;

	public override void Trigger(){
		Execute();
		DoVisualEffect();
	}

	protected override void Execute(){
		_executing = true;
		_nextDamage = Time.time;
		_endTime = Time.time + DurationTime + Random.Range(-1.0f, 1.0f);
	}

	protected virtual void Update(){
		if(!_executing)
			return;

		if(_endTime <= Time.time){
			_executing = false;
			Terminate();
			return;
		}

		if(_nextDamage <= Time.time){
			base.Execute();
			_nextDamage = Time.time + DamageInterval;
		}
	}
}
