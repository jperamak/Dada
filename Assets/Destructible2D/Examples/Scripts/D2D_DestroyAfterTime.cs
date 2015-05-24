using UnityEngine;

[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Destroy After Time")]
public class D2D_DestroyAfterTime : MonoBehaviour
{
	public float Seconds = 10.0f;
	
	public bool FadeOut;
	
	public float FadeSeconds = 1.0f;
	
	private SpriteRenderer spriteRenderer;
	
	protected virtual void Update()
	{
		Seconds -= Time.deltaTime;
		
		if (Seconds <= 0.0f)
		{
			Destroy(gameObject);
		}
		else
		{
			if (FadeOut == true)
			{
				if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
				
				var color = spriteRenderer.color;
				
				color.a = Mathf.Clamp01(D2D_Helper.Divide(Seconds, FadeSeconds));
				
				spriteRenderer.color = color;
			}
		}
	}
}