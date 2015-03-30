using UnityEngine;
using System.Collections.Generic;

public class AlignWithAttractorPoint : MonoBehaviour {

	private List<Transform> _points = new List<Transform>();
	private Rigidbody2D _rigidbody;
	private float _prevGravityScale;

	void Awake(){
		_rigidbody = GetComponent<Rigidbody2D>();
	}

	public void AddPoint(Transform point){
		if(!_points.Contains(point)){
			_points.Add(point);

			//If it's the first gravity field, ignore the earth's gravity attraction
			if(_points.Count == 1){
				_prevGravityScale = _rigidbody.gravityScale;
				//Debug.Log("Set "+name+" scale to "+_prevGravityScale);
				_rigidbody.gravityScale = 0;
			}

		}
	}

	public void RemovePoint(Transform point){
		if(_points.Contains(point)){
			_points.Remove(point);


			//If there aren't gravity fields, put back the earth's gravity
			if(_points.Count == 0){
				//Debug.Log("Reset "+name+" to gravity "+_prevGravityScale);
				_rigidbody.gravityScale = _prevGravityScale;
				transform.rotation = Quaternion.Euler (0,0,0);
				//transform.rotation = Quaternion.FromToRotation (transform.up, Vector2.up);
			}
		}
	}

	void Update () {
		if(_points.Count == 0)
			return;

		Vector2 down = Vector2.zero;
		for(int i=0;i<_points.Count; i++){
			down += (Vector2)(transform.position - _points[i].transform.position);
		}
		down = down / _points.Count;
		down.Normalize();


		Quaternion targetRotation = Quaternion.FromToRotation (Vector2.up, down);
		transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, 10*Time.deltaTime);
		//transform.rotation = targetRotation;

	}
}
