using UnityEngine;

[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Top Down Car")]
public class D2D_TopDownCar : MonoBehaviour
{
	public D2D_TopDownWheel FrontLeftWheel;
	
	public D2D_TopDownWheel FrontRightWheel;
	
	public D2D_TopDownWheel BackLeftWheel;
	
	public D2D_TopDownWheel BackRightWheel;
	
	public float Throttle;
	
	public float ThrottleLimit = 10.0f;
	
	public float SteeringAngle;
	
	public float SteeringLimit = 30.0f;
	
	protected virtual void Update()
	{
		Throttle = Input.GetAxis("Vertical") * ThrottleLimit;
		
		if ( FrontLeftWheel != null)  FrontLeftWheel.SurfaceSpeed = Throttle;
		if (FrontRightWheel != null) FrontRightWheel.SurfaceSpeed = Throttle;
		if (  BackLeftWheel != null)   BackLeftWheel.SurfaceSpeed = Throttle;
		if ( BackRightWheel != null)  BackRightWheel.SurfaceSpeed = Throttle;
		
		SteeringAngle = Input.GetAxis("Horizontal") * SteeringLimit;
		
		if ( FrontLeftWheel != null)  FrontLeftWheel.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -SteeringAngle);
		if (FrontRightWheel != null) FrontRightWheel.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -SteeringAngle);
	}
}