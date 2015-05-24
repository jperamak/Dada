using UnityEngine;

[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Top Down Thruster")]
public class D2D_TopDownThruster : MonoBehaviour
{
	public Rigidbody2D body;
	
	public float Throttle;
	
	public float MoveSpeed = 50.0f;
	
	public float TurnSpeed = 5.0f;
	
	protected virtual void FixedUpdate()
	{
		transform.localScale = new Vector3(Throttle, Throttle, Throttle);
		
		if (body != null)
		{
			body.velocity        += Throttle * MoveSpeed * Time.fixedDeltaTime * (Vector2)transform.up;
			body.angularVelocity += Throttle * TurnSpeed * Time.fixedDeltaTime;
		}
	}
}