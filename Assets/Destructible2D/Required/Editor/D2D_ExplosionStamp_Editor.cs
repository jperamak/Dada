using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(D2D_ExplosionStamp))]
public class D2D_ExplosionStamp_Editor : D2D_Editor<D2D_ExplosionStamp>
{
	protected override void OnInspector()
	{
		DrawDefault("Layers");
		DrawDefault("StampTex");
		DrawDefault("Hardness");
		DrawDefault("Size");
		DrawDefault("AngleOffset");
		DrawDefault("AngleRandomness");
		
		Separator();
		
		DrawDefault("HasExploded");
	}
}