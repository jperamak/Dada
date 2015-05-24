using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(D2D_SwappableSprite))]
public class D2D_SwappableSprite_Editor : D2D_Editor<D2D_SwappableSprite>
{
	protected override void OnInspector()
	{
		DrawDefault("DamageLevels");
	}
}