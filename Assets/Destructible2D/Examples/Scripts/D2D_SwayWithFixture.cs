using UnityEngine;

[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Sway With Fixture")]
public class D2D_SwayWithFixture : MonoBehaviour
{
	public D2D_Fixture RequiredFixture;
	
	public float Age;
	
	public float TimeScale = 1.0f;
	
	public float Sway = 1.0f;
	
	protected virtual void Update()
	{
		if (RequiredFixture != null)
		{
			Age += TimeScale * Time.deltaTime;
			
			transform.localRotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Sin(Age) * Sway);
		}
	}
}