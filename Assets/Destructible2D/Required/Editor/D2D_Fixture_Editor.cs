using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(D2D_Fixture))]
public class D2D_Fixture_Editor : D2D_Editor<D2D_Fixture>
{
	protected override void OnInspector()
	{
		if (Any(t => D2D_Helper.GetComponentUpwards<D2D_DestructibleSprite>(t.transform) == null))
		{
			EditorGUILayout.HelpBox("A parent of this GameObject should have the D2D_DestructibleSprite component", MessageType.Error);
		}
		
		DrawDefault("Offset");
		DrawDefault("Target");
		DrawDefault("Threshold");
		DrawDefault("Pinned");
	}
}