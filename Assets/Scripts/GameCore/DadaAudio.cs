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


    public static SoundEffect GetSoundEffect(SoundEffect e)
    {
        if (e == null)
            return null;
        var r = GameObject.Find(e.name + "(Clone)");
        if (r != null)
            return r.GetComponent<SoundEffect>();
        else 
            return Instantiate(e) as SoundEffect;
    }
}
