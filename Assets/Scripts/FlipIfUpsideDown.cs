using UnityEngine;
using System.Collections;

public class FlipIfUpsideDown : MonoBehaviour {
	
	public float MaxUpsideDown = 2.5f;
	public float FlipForce = 2.0f;

	private float _timeUpsideDown = 0;

		
	
	void Update(){
		
		float zAngle = transform.rotation.eulerAngles.z;
		if(zAngle > 100 && zAngle < 260)
			_timeUpsideDown += Time.deltaTime;
		else if(zAngle < 10)
			_timeUpsideDown = 0;
			
		if(_timeUpsideDown > MaxUpsideDown){
			Flip();
			_timeUpsideDown = 0;
		}

	}
	
	
	void Flip(){
		float multiplier = Mathf.Clamp((int)(_timeUpsideDown/MaxUpsideDown) * 20.0f * transform.localScale.x,0,100.0f);
		
		transform.GetComponent<Rigidbody2D>().AddForce(-transform.up * (FlipForce+multiplier), ForceMode2D.Impulse);
	}
}
