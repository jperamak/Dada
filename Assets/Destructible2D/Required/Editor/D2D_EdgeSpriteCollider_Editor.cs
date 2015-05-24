using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(D2D_EdgeSpriteCollider))]
public class D2D_EdgeSpriteCollider_Editor : D2D_SpriteCollider_Editor<D2D_EdgeSpriteCollider>
{
	private static bool hideColliders;
	
	protected override void OnInspector()
	{
		base.OnInspector();
		
		Separator();
		
		EditorGUI.BeginChangeCheck();
		{
			DrawDefault("CellSize");
			DrawDefault("Detail");
			DrawDefault("Binary");
		}
		if (EditorGUI.EndChangeCheck() == true)
		{
			Each(t => t.MarkAsDirty());
		}
	}
}