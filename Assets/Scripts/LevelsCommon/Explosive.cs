using UnityEngine;
using System.Collections;

public class Explosive : Damageable {

	private Explosion _explosionEffect;
	private Projectile _projectile;

	void Start(){
		_explosionEffect = GetComponent<Explosion>();
		_projectile = GetComponent<Projectile>();

		if(_projectile != null)
			_explosionEffect.Owner = _projectile.Owner;

	}

	protected override void OnDestroyed (){


		_explosionEffect.Trigger();

		base.OnDestroyed ();
	}

}
