using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(D2D_PolygonColliders))]
public class D2D_PolygonColliders_Editor : D2D_Collider_Editor<D2D_PolygonColliders>
{
	private static bool hideColliders;
	
	protected override void OnInspector()
	{
		base.OnInspector();
		
		BeginChangeCheck();
		{
			DrawDefault("CellSize");
			DrawDefault("Tolerance");
		}
		EndChangeCheck();
	}
}