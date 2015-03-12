using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public Player PlayerOwner{get; private set;}
	public GameObject Owner{
		get{ return _owner;} 
		set{
			_owner = value;
			if(value != null){
				Hero hero = _owner.GetComponent<Hero>();
				if(hero != null)
					PlayerOwner = hero.PlayerInstance;
			}
			else
				PlayerOwner = null;
		}
	}

	protected Player _playerOwner;
	protected GameObject _owner;
	protected Effect[] _effects;
	protected int _activeEffects;

	void Awake(){
		_effects = GetComponents<Effect>();
		if(_effects != null)
			_activeEffects = _effects.Length;
	}

	protected virtual void TriggerEffects(){

		if(_effects != null){
			for(int i=0;i<_effects.Length;i++){
				_effects[i].Owner = Owner;
				_effects[i].OnEnd += OnEffectFinshed;
				_effects[i].Trigger();
			}
		}
	}

	void FixedUpdate (){
		Vector2 velocity = gameObject.GetComponent<Rigidbody2D>().velocity;
		float angle = Mathf.Atan2( velocity.y, velocity.x );
		transform.eulerAngles = new Vector3(0, 0, angle *  Mathf.Rad2Deg);
	}

	protected void OnEffectFinshed(){

		_activeEffects--;
		Debug.Log(name+ " activeEffects: "+_activeEffects);
		if(_activeEffects == 0)
			Destroy(this.gameObject);
	}
}
