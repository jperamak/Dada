using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(D2D_Splittable))]
public class D2D_Splittable_Editor : D2D_Editor<D2D_Splittable>
{
	protected override void OnInspector()
	{
		DrawDefault("Threshold");
		
		BeginError(Any(t => t.MinPixels <= 0));
		{
			DrawDefault("MinPixels");
		}
		EndError();
		
		DrawDefault("SplitOrder");
	}
}