using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(D2D_AutoPolygonCollider))]
public class D2D_AutoPolygonCollider_Editor : D2D_Collider_Editor<D2D_AutoPolygonCollider>
{
	protected override void OnInspector()
	{
		base.OnInspector();
	}
}