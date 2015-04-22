using UnityEngine;
using System.Collections;

public class Ghost : MonoBehaviour {
	
	private float _distance;
	private float _timeEnd;
	private Vector2 _direction;
	private Vector2 _initialPos;
	private bool _going = false;
	private bool _approaching = false;
	private float _passedTime = 0;
	private float _totTime = 0; 
	
	private Animator _anim;

	void Start(){
		_anim = GetComponentInChildren<Animator>();
	}

	public void Goto(Vector2 target, float seconds){
		_initialPos = transform.position;
		_direction = (target - _initialPos);
		_distance = _direction.magnitude;
		_direction.Normalize();
		_totTime = seconds;
		_going = true;

	}

	public void SetColor(Color c){
		SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
		renderer.color = new Color(c.r, c.g, c.b, 0.5f);

		ParticleSystem particles = GetComponentInChildren<ParticleSystem>();
		particles.startColor = c;
	}
	
	private void KillGhost(){
		_going = false;
	}

	void Update(){
		if(_going){
			_passedTime += Time.deltaTime;
			float dist = Mathf.Lerp(0,_distance,_passedTime/_totTime);
			transform.position = _initialPos + (_direction * dist);
			if( _totTime - _passedTime < 1 && !_approaching){
				_approaching = true;
				_anim.SetTrigger("Disappear");
				Invoke("KillGhost",1.0f);
			}
		}
	}


}
