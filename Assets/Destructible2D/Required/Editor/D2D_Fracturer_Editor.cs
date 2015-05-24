using UnityEngine;
using UnityEditor;

public abstract class D2D_Fracturer_Editor<T> : D2D_Editor<T>
	where T : D2D_Fracturer
{
	protected override void OnInspector()
	{
		BeginError(Any(t => t.DamageLimit <= 0.0f));
		{
			DrawDefault("DamageLimit");
		}
		EndError();
		
		BeginError(Any(t => t.Count <= 0));
		{
			DrawDefault("Count");
		}
		EndError();
	}
}