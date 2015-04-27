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
		
		BeginError(Any(t => t.SourceMaterial == null));
		{
			DrawDefault("SourceMaterial");
		}
		EndError();
		
		if (Any(t => t.SourceMaterial != null && AssetDatabase.Contains(t.SourceMaterial) == false))
		{
			EditorGUILayout.HelpBox("This material isn't an asset, so you won't be able to store this Destructible Sprite in a prefab.", MessageType.Warning);
		}
		
		if (Any(t => t.SourceMaterial != null && t.SourceMaterial.HasProperty("_AlphaTex") == false))
		{
			EditorGUILayout.HelpBox("This material lacks the _AlphaTex property, so it will not render correctly.", MessageType.Error);
		}
		
		DrawDefault("Sharpness");
		
		EditorGUILayout.Separator();
		
		DrawDefault("ColliderType");
		
		if (Any(t => t.ColliderType == D2D_SpriteColliderType.AutoPolygon && t.AlphaTex != null && t.AlphaTex.width * t.AlphaTex.height > 400 * 400))
		{
			if (HelpButton("Your Alpha Tex is quite large, and it may run slowly with this Collider Type. Consider lowering its resolution.", MessageType.Warning, "Halve Alpha Tex", 50.0f) == true)
			{
				Each(t => t.HalveAlphaTex());
			}
		}
		
		EditorGUILayout.Separator();
		
		DrawDefault("AllowSplit");
		
		if (Any(t => t.AllowSplit == true && t.AlphaTex != null && t.AlphaTex.width * t.AlphaTex.height > 400 * 400))
		{
			if (HelpButton("Your Alpha Tex is quite large, and it may run slowly with Allow Split. Consider lowering its resolution.", MessageType.Warning, "Halve Alpha Tex", 50.0f) == true)
			{
				Each(t => t.HalveAlphaTex());
			}
		}
		
		if (Any(t => t.AllowSplit == true))
		{
			DrawDefault("SplitMinPixels");
			DrawDefault("SplitThreshold");
			DrawDefault("SplitOrder");
		}
		
		if (Targets.Length == 1 && AssetDatabase.Contains(Target) == false)
		{
			EditorGUILayout.Separator();
			
			EditorGUI.BeginDisabledGroup(true);
			{
				EditorGUILayout.IntField("Solid Pixel Count", Target.SolidPixelCount);
				EditorGUILayout.IntField("Original Solid Pixel Count", Target.OriginalSolidPixelCount);
				EditorGUILayout.Slider("Solid Pixel Ratio", Target.SolidPixelRatio, 0.0f, 1.0f);
			}
			EditorGUI.EndDisabledGroup();
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