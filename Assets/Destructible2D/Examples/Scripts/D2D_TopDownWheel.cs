using UnityEngine;

[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Top Down Wheel")]
public class D2D_TopDownWheel : MonoBehaviour
{
	public Rigidbody2D body;
	
	public float SurfaceSpeed;
	
	protected virtual void FixedUpdate()
	{
		if (body != null)
		{
			body.AddForceAtPosition(transform.up * SurfaceSpeed, transform.position);
		}
	}
}