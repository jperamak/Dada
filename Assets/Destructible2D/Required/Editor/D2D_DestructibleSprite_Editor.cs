using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(D2D_DestructibleSprite))]
public class D2D_DestructibleSprite_Editor : D2D_Editor<D2D_DestructibleSprite>
{
	protected override void OnInspector()
	{
		DrawAlphaTex();
		DrawDefault("DensityTex");
		
		DrawDefault("Sharpness");
		
		DrawDefault("Indestructible");
		
		DrawDefault("Binary");
		
		Separator();
		
		BeginError(Any(t => t.SplitDepth < 0));
		{
			DrawDefault("SplitDepth");
		}
		EndError();
		
		BeginError(Any(t => t.MinSplitPixels < 0));
		{
			DrawDefault("MinSplitPixels");
		}
		EndError();
		
		if (Targets.Length == 1 && AssetDatabase.Contains(Target) == false)
		{
			Separator();
			
			EditorGUI.BeginDisabledGroup(true);
			{
				EditorGUILayout.IntField("Solid Pixel Count", Target.SolidPixelCount);
				EditorGUILayout.IntField("Original Solid Pixel Count", Target.OriginalSolidPixelCount);
				EditorGUILayout.Slider("Solid Pixel Ratio", Target.SolidPixelRatio, 0.0f, 1.0f);
			}
			EditorGUI.EndDisabledGroup();
			
			var spriteRenderer = Target.GetComponent<SpriteRenderer>();
			
			if (spriteRenderer != null)
			{
				var material = spriteRenderer.sharedMaterial;
				
				if (material != null)
				{
					if (material.HasProperty("_AlphaTex") == false)
					{
						EditorGUILayout.HelpBox("Material does not have a _AlphaTex texture property. It is required for D2D_DestructibleSprite.", MessageType.Warning);
					}
					
					if (material.HasProperty("_AlphaScale") == false)
					{
						EditorGUILayout.HelpBox("Material does not have a _AlphaScale texture property. It is required for D2D_DestructibleSprite.", MessageType.Warning);
					}
					
					if (material.HasProperty("_AlphaOffset") == false)
					{
						EditorGUILayout.HelpBox("Material does not have a _AlphaOffset texture property. It is required for D2D_DestructibleSprite.", MessageType.Warning);
					}
				}
			}
		}
	}
	
	private void DrawAlphaTex()
	{
		var newAlphaTex = (Texture2D)EditorGUI.ObjectField(D2D_Helper.Reserve(), "Alpha Tex", Target.AlphaTex, typeof(Texture2D), false);
		
		if (newAlphaTex != Target.AlphaTex)
		{
			D2D_Helper.Record(Targets, "Replace Destructible Sprite Alpha");
			
			foreach (var t in Targets)
			{
				t.ReplaceAlphaWith(newAlphaTex); t.RecalculateOriginalSolidPixelCount(); D2D_Helper.SetDirty(t);
			}
		}
	}
}