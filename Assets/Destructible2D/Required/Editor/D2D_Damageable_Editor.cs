using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(D2D_Damageable))]
public class D2D_Damageable_Editor : D2D_Editor<D2D_Damageable>
{
	protected override void OnInspector()
	{
		BeginError(Any(t => t.Damage < 0.0f));
		{
			DrawDefault("Damage");
		}
		EndError();
		
		BeginError(Any(t => t.Age < 0.0f));
		{
			DrawDefault("Age");
		}
		EndError();
		
		BeginError(Any(t => t.ActivateDelay < 0.0f));
		{
			DrawDefault("ActivateDelay");
		}
		EndError();
		
		Separator();
		
		DrawDefault("AllowDestruction");
		
		if (Any(t => t.AllowDestruction == true))
		{
			BeginIndent();
			{
				DrawDefault("DamageLimit");
				DrawDefault("ReplaceWith");
			}
			EndIndent();
		}
		
		Separator();
	}
}