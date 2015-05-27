using UnityEngine;
using System.Collections;


public class DelayAnimation : MonoBehaviour {
	
	void Start () {
		Animator _anim = GetComponent<Animator>();

		if(_anim != null){
			int hash = _anim.GetCurrentAnimatorStateInfo(0).shortNameHash;
			_anim.Play(hash, 0,  Random.Range(0f,1f));
		}

		else{
			Animation _animation = GetComponent<Animation>();
			string name = _animation.clip.name;
			_animation[name].normalizedTime = Random.Range(0f,1f);
		}
	}
	

}
