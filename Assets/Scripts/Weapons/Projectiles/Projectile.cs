using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public GameObject VisualEffectOnTrigger;

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

	public virtual void TriggerEffects(){

		if(_effects != null){
			for(int i=0;i<_effects.Length;i++){
				_effects[i].Owner = Owner;
				_effects[i].OnEnd += OnEffectFinshed;
				_effects[i].Trigger();
			}
		}
	}
	
	protected void OnEffectFinshed(){

		_activeEffects--;
		if(_activeEffects == 0)
			Destroy(this.gameObject);
	}
}
