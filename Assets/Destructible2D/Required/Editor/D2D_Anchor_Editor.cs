using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(D2D_Anchor))]
public class D2D_Anchor_Editor : D2D_Editor<D2D_Anchor>
{
	protected override void OnInspector()
	{
		DrawDefault("Radius");
	}
}