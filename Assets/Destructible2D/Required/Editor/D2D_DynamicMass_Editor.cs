using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(D2D_DynamicMass))]
public class D2D_DynamicMass_Editor : D2D_Editor<D2D_DynamicMass>
{
	protected override void OnInspector()
	{
		DrawDefault("MassPerPixel");
	}
}