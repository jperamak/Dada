using UnityEngine;
using System.Collections;

public class FadeOutOfExistence : MonoBehaviour {

	public float secondsToStartFading;
	public float secondsForFading;
    public bool destroy = true;
	private SpriteRenderer spriteRend;
	private float awakeTime;

	void Awake () 
	{
		awakeTime = Time.time;
        if ( destroy)
            Destroy(gameObject.transform.parent.gameObject, secondsToStartFading + secondsForFading);
		spriteRend = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		float timeIntoFading = Time.time - awakeTime - secondsToStartFading;

		if (timeIntoFading > 0.0f && secondsForFading > 0.0f)
			SetAlpha( 1.0f - timeIntoFading/secondsForFading);
	}


	void SetAlpha( float value ) // from 0 .. 1
	{
		if (spriteRend == null)
				return;
		Color color = new Color( spriteRend.color.r, spriteRend.color.g, spriteRend.color.b, value);
		spriteRend.material.color = color;
	}
			             


}
