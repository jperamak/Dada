using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(D2D_EdgeColliders))]
public class D2D_EdgeColliders_Editor : D2D_Collider_Editor<D2D_EdgeColliders>
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