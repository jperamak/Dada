using UnityEngine;
using System.Collections;

public class DadaAudio : MonoBehaviour {

	public static void PlaySound(AudioClip clip){
		AudioSource.PlayClipAtPoint(clip, Vector2.zero, 1.0f);
	}

	public static void PlayRandom(AudioClip[] clip){
		int index = Random.Range(0,clip.Length);
		PlaySound(clip[index]);
	}
}
