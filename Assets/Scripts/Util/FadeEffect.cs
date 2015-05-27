using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class FadeEffect : MonoBehaviour {

	public float StartGradient = 0.0f;
	public float EndGradient = 1.0f;
	public float FadeTime = 1.5f;
	public float Delay = 0.0f;
	public bool Loop = false;
	public bool FadeOnAwake = false;

	public bool IsFading{get{return _fade;}}

	private float _waitedDelay = 0;
	private float _progress = 0;
	private bool _fade = false;
	private CanvasGroup _group;

	void Awake () {
		_fade = FadeOnAwake;
		_group = GetComponent<CanvasGroup>();
		_group.alpha = StartGradient;
	}

	public void Stop(float initialFade, float endingFade){
		StartGradient = initialFade;
		EndGradient = endingFade;
		_group.alpha = initialFade;
		_progress = 0;
		_waitedDelay = 0;
		_fade = false;
	}

	public void Fade(){
		_fade = true;
	}

	public void Fade(float start, float end, float delay, float time){
		StartGradient = start;
		EndGradient = end;
		Delay = delay;
		FadeTime = time;
		_group.alpha = start;
		_progress = 0;
		_waitedDelay = 0;
		_fade = true;
	}

	public void FadeOut(){
		float t = EndGradient;
		EndGradient = StartGradient;
		StartGradient = t;
		_progress = 0;
		_waitedDelay = 0;
		_fade = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(_fade){
			if(Delay != 0 && _waitedDelay < Delay){
				_waitedDelay += Time.deltaTime;
				return;
			}
			if(_group.alpha == EndGradient){
				if(Loop){
					FadeOut();
				}
			}

			else{
				_progress += Time.deltaTime;
				_group.alpha = Mathf.Lerp(StartGradient,EndGradient,_progress/FadeTime);
			}
		}
	}





}
