using UnityEngine;
using System.Collections;

public class FrogBomb : Projectile {
	
	public float firstPhaseLength = 5.0f;
	public float secondPhaseLength = 2.0f;

	private SpriteRenderer _renderer;
	private bool exploded = false;

	private float _detonationTime;


	void Start(){
		_renderer = transform.GetComponentInChildren<SpriteRenderer>();
		_detonationTime = Time.time + firstPhaseLength + secondPhaseLength;
		Invoke( "AboutToDetonate", firstPhaseLength );

	}
	

	private void AboutToDetonate(){
		StartCoroutine(TickTack());
	}

	private void Explode(){
		TriggerEffects();
		exploded = true;
	}
	
	private IEnumerator TickTack(){
		while(!exploded){
			if(_renderer.color == Color.white)
				_renderer.color = Color.red;
			else
				_renderer.color = Color.white;
			
			if(Time.time >= _detonationTime)
				Explode();
			yield return new WaitForSeconds(0.25f);
		}
	}
	
	
	
}
