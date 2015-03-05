using UnityEngine;
using System.Collections.Generic;

public class DamageOnContact : MonoBehaviour {

	public float Damage;
	public float CooldownTime;

	private Dictionary<int, float> _hits;

	void Start(){
		_hits = new Dictionary<int, float>();
	}

	void ApplyDamage(Collision2D coll){
		int id = coll.gameObject.GetInstanceID();
		
		//target already hit
		if(_hits.ContainsKey(id) && _hits[id] > Time.time - CooldownTime)
			return;
		
		PlayerHealth health = coll.gameObject.GetComponent<PlayerHealth>();
		if(health != null){
			Debug.Log("Damage "+id+" "+health.name);
			health.TakeDamage(null);
			_hits[id] = Time.time;
		}
	}

	void OnCollisionEnter2D(Collision2D coll){
		ApplyDamage(coll);
	}

	void OnCollisionStay2D(Collision2D coll){
		ApplyDamage(coll);
	}
}
