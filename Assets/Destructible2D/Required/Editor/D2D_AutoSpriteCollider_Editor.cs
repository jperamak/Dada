using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(D2D_AutoSpriteCollider))]
public class D2D_AutoSpriteCollider_Editor : D2D_SpriteCollider_Editor<D2D_AutoSpriteCollider>
{
	protected override void OnInspector()
	{
		base.OnInspector();
	}
}