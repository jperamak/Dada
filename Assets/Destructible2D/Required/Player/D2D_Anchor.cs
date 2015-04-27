using UnityEngine;

[AddComponentMenu("Destructible 2D/D2D Anchor")]
public class D2D_Anchor : MonoBehaviour
{
	public float Radius = 1.0f;
	
	public float ScaledRadius
	{
		get
		{
			return Radius * Mathf.Max(transform.lossyScale.x, Mathf.Max(transform.lossyScale.y, transform.lossyScale.z));
		}
	}
	
#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		var c = transform.position;
		var r = ScaledRadius;
		var s = Mathf.PI * 2.0f / 36.0f;
		
		for (var i = 0; i < 36; i++)
		{
			var a = i * s;
			var b = a + s;
			
			Gizmos.DrawLine(c + new Vector3(Mathf.Sin(a), Mathf.Cos(a), 0.0f) * r, c + new Vector3(Mathf.Sin(b), Mathf.Cos(b), 0.0f) * r);
		}
	}
#endif
}