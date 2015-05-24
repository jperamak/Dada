using UnityEngine;

[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Top Down Spaceship")]
public class D2D_TopDownSpaceship : MonoBehaviour
{
	public D2D_TopDownThruster LeftThruster;
	
	public D2D_TopDownThruster RightThruster;
	
	public float LeftThrottle;
	
	public float RightThrottle;
	
	public D2D_TopDownGun LeftGun;
	
	public D2D_TopDownGun RightGun;
	
	protected virtual void Update()
	{
		LeftThrottle  = Input.GetAxis("Vertical") + Mathf.Abs(Mathf.Max(0.0f, Input.GetAxis("Horizontal")));
		RightThrottle = Input.GetAxis("Vertical") + Mathf.Abs(Mathf.Min(0.0f, Input.GetAxis("Horizontal")));
		
		if ( LeftThruster != null)  LeftThruster.Throttle = LeftThrottle;
		if (RightThruster != null) RightThruster.Throttle = RightThrottle;
		
		if (LeftGun  != null)  LeftGun.IsFiring = Input.GetAxis("Jump") > 0.0f;
		if (RightGun != null) RightGun.IsFiring = Input.GetAxis("Jump") > 0.0f;
	}
}