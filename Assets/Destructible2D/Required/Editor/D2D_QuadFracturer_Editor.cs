using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(D2D_QuadFracturer))]
public class D2D_QuadFracturer_Editor : D2D_Fracturer_Editor<D2D_QuadFracturer>
{
	protected override void OnInspector()
	{
		base.OnInspector();
		
		Separator();
		
		DrawDefault("Irregularity");
	}
}