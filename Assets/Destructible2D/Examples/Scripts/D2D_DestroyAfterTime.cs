using UnityEngine;

[AddComponentMenu("Destructible 2D/D2D Destroy After Time")]
public class D2D_DestroyAfterTime : MonoBehaviour
{
	public float Seconds = 10.0f;
	
	protected virtual void Update()
	{
		Seconds -= Time.deltaTime;
		
		if (Seconds <= 0.0f)
		{
			Destroy(gameObject);
		}
	}
}