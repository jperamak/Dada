using UnityEngine;
using System.Collections;

public class Croco : MonoBehaviour {

	public float MaxUpsideDown = 2.5f;
	public float FlipForce = 2.0f;
	public float CrocoHp = 100.0f;

	private float _timeUpsideDown = 0;
	private int _parts = 0;
	private Transform _head;
	void Start(){

		_head = transform.FindChild("Head");
		int childrenNum = transform.childCount;
		_parts = childrenNum;


		for(int i=0;i<childrenNum;i++){
			Damageable dmg = transform.GetChild(i).gameObject.AddComponent<Damageable>();
			dmg.maxHitpoints = CrocoHp * transform.localScale.x;
			dmg.currentHitpoints = CrocoHp * transform.localScale.x;
			dmg.Destroyed += PartDestroyed;
		}
	}

	void Update(){

		if(_head != null && _head.GetComponent<Rigidbody2D>() != null){
			float zAngle = _head.transform.rotation.eulerAngles.z;
			if(zAngle > 100 && zAngle < 260)
				_timeUpsideDown += Time.deltaTime;
			else if(zAngle < 10)
				_timeUpsideDown = 0;

			if(_timeUpsideDown > MaxUpsideDown){
				Flip();
				_timeUpsideDown = 0;
			}
		}
	}

	void PartDestroyed(){
		_parts--;
		if(_parts <= 0)
			Destroy(gameObject);
	}

	void Flip(){
		float multiplier = Mathf.Clamp((int)(_timeUpsideDown/MaxUpsideDown) * 20.0f * transform.localScale.x,0,100.0f);

		_head.GetComponent<Rigidbody2D>().AddForce(-_head.transform.up * (FlipForce+multiplier), ForceMode2D.Impulse);
	}
}
