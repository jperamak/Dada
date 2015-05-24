using UnityEngine;
using UnityEditor;

public abstract class D2D_SpriteCollider_Editor<T> : D2D_Editor<T>
	where T : D2D_SpriteCollider
{
	protected override void OnInspector()
	{
		DrawDefault("IsTrigger");
		DrawDefault("Material");
		
		foreach (var t in Targets)
		{
			t.UpdateColliderSettings();
		}
	}
}