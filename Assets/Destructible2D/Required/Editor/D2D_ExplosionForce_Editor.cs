using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(D2D_ExplosionForce))]
public class D2D_ExplosionForce_Editor : D2D_Editor<D2D_ExplosionForce>
{
	protected override void OnInspector()
	{
		DrawDefault("Layers");
		DrawDefault("Radius");
		DrawDefault("Force");
		DrawDefault("Samples");
		
		Separator();
		
		DrawDefault("HasExploded");
	}
}