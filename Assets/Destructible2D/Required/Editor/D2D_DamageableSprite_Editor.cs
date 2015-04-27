using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(D2D_DamageableSprite))]
public class D2D_DamageableSprite_Editor : D2D_Editor<D2D_DamageableSprite>
{
	protected override void OnInspector()
	{
		DrawDefault("Damage");
		
		EditorGUILayout.Separator();
		
		DrawDefault("AllowDestruction");
		
		if (Any(t => t.AllowDestruction == true))
		{
			DrawDefault("DamageLimit");
			DrawDefault("ReplaceWith");
		}
		
		EditorGUILayout.Separator();
		
		DrawDefault("DamageLevels");
	}
}