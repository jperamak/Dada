using UnityEngine;
using UnityEditor;

public abstract class D2D_Collider_Editor<T> : D2D_Editor<T>
	where T : D2D_Collider
{
	private static bool hide;
	
	protected override void OnInspector()
	{
		DrawHide();
		DrawDefault("IsTrigger");
		DrawDefault("Material");
		
		foreach (var t in Targets)
		{
			t.UpdateColliderSettings();
			t.SetHideFlags(hide);
		}
		
		EditorGUILayout.Separator();
	}
	
	protected void BeginChangeCheck()
	{
		EditorGUI.BeginChangeCheck();
	}
	
	protected void EndChangeCheck()
	{
		if (EditorGUI.EndChangeCheck() == true)
		{
			foreach (var t in Targets)
			{
				if (t != null)
				{
					t.MarkAsDirty();
				}
			}
		}
	}
	
	private void DrawHide()
	{
		var position = D2D_Helper.Reserve();
		
		if (D2D_Helper.BaseRectSet == true)
		{
			position.x     = D2D_Helper.BaseRect.x;
			position.width = D2D_Helper.BaseRect.width;
		}
		
		hide = EditorGUI.Toggle(position, "Hide", hide);
	}
}