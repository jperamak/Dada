using UnityEngine;
using System.Collections;

public class FrogBomb : Projectile {
	
	public float firstPhaseLength = 5.0f;
	public float secondPhaseLength = 2.0f;
	public float jumpForce;

	public SoundEffect CroakSound;
	public SoundEffect AboutToExplodeSound;

	private bool exploded = false;

	private float _detonationTime;


	void Start(){
		CroakSound = DadaAudio.GetSoundEffect(CroakSound);
		AboutToExplodeSound = DadaAudio.GetSoundEffect(AboutToExplodeSound);
		
		_detonationTime = Time.time + firstPhaseLength + secondPhaseLength;
		Invoke( "AboutToDetonate", firstPhaseLength );
		StartCoroutine( Croak() );

	}
	

	private void AboutToDetonate(){
		StartCoroutine(TickTack());
		AboutToExplodeSound.PlayEffect();
	}

	private void Explode(){
		TriggerEffects();
		exploded = true;
	}

	// Croak and jump
	private IEnumerator Croak(){
		while(!exploded){
			CroakSound.PlayEffect();
			this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f,jumpForce), ForceMode2D.Force);
			yield return new WaitForSeconds(2.0f);
		}
	}
	
	private IEnumerator TickTack(){
		while(!exploded){
			float stepTime = 0.025f;
			float scaleIncrease = 0.65f;
			float addToScale = scaleIncrease / (secondPhaseLength / stepTime);
			transform.localScale = new Vector3(transform.localScale.x + addToScale,
			                                   transform.localScale.y + addToScale, 0f);
			
			if(Time.time >= _detonationTime)
				Explode();
			yield return new WaitForSeconds(stepTime);
		}
	}
	
	
	
}
