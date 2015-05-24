using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Follow")]
public class D2D_Follow : MonoBehaviour
{
	public Transform Target;
	
	public Vector3 Offset;
	
	protected virtual void Update()
	{
		if (Target != null)
		{
			D2D_Helper.SetPosition(transform, Target.transform.position + Offset);
		}
	}
}