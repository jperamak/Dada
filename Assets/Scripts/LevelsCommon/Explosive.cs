using UnityEngine;
using System.Collections;

public class Explosive : Damageable {

	private Explosion _explosionEffect;

	void Start(){
		_explosionEffect = GetComponent<Explosion>();
	}

	protected override void OnDestroyed (){


		_explosionEffect.Trigger();

		base.OnDestroyed ();
	}

}
