using UnityEngine;
using System.Collections;

public class FadeOutOfExistence : MonoBehaviour {

	public float secondsToStartFading;
	public float secondsForFading;

	private SpriteRenderer spriteRend;

	void Awake () 
	{
		//Invoke ("StartFading", secondsToStartFading);
		SpriteRenderer spriteRend = GetComponent<SpriteRenderer>();
		Debug.Log (spriteRend.color);
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (spriteRend != null)
			SetAlpha( 0.1f);
	
	}

	void StartFading()
	{

		Destroy (gameObject);
	}

	void SetAlpha( float value ) // from 0 .. 1
	{
		//spriteRend.material.color.a = value;
		//Color color = new Color( 0f,0f,0f); //spriteRend.color.r, spriteRend.color.g, spriteRend.color.b, value);
		//spriteRend.material.color = color;
	}
			             


}
