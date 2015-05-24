using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(D2D_DetachableSprite))]
public class D2D_DetachableSprite_Editor : D2D_Editor<D2D_DetachableSprite>
{
	protected override void OnInspector()
	{
		if (Any(t => t.GetComponentInChildren<D2D_Fixture>() == null))
		{
			EditorGUILayout.HelpBox("This GameObject has no D2D_Fixture children.", MessageType.Warning);
		}
		
		BeginError(Any(t => t.OldCollider == null));
		{
			DrawDefault("OldCollider");
		}
		EndError();
		
		BeginError(Any(t => t.NewCollider == null));
		{
			DrawDefault("NewCollider");
		}
		EndError();
	}
}