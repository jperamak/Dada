using UnityEngine;

[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Spectator")]
public class D2D_Spectator : MonoBehaviour
{
	public float Speed = 10.0f;
	
	public float Dampening = 10.0f;
	
	public Vector3 Velocity;
	
	protected virtual void Update()
	{
		var targeteVelocity = Vector3.zero;
		
		targeteVelocity.x = Input.GetAxisRaw("Horizontal");
		targeteVelocity.y = Input.GetAxisRaw("Vertical");
		
		if (targeteVelocity.magnitude > 0.01f)
		{
			targeteVelocity = targeteVelocity.normalized * Speed;
		}
		
		Velocity = D2D_Helper.Dampen3(Velocity, targeteVelocity, Dampening, Time.deltaTime, 0.1f);
		
		transform.Translate(Velocity * Time.deltaTime);
	}
}