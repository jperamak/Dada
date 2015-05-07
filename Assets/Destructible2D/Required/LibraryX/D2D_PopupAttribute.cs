using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class D2D_PopupAttribute : PropertyAttribute
{
	public GUIContent[] Names;
	
	public int[] Values;
	
	public D2D_PopupAttribute(params int[] newValues)
	{
		Names = newValues.Select(v => new GUIContent(v.ToString())).ToArray();
		
		Values = newValues.Select(v => v).ToArray();
	}
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(D2D_PopupAttribute))]
public class D2D_PopupDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (D2D_Helper.BaseRectSet == true)
		{
			position.x = D2D_Helper.BaseRect.x;
			position.width = D2D_Helper.BaseRect.width;
		}
		
		var Attribute = (D2D_PopupAttribute)attribute;
		
		EditorGUI.IntPopup(position, property, Attribute.Names, Attribute.Values, label);
	}
}
#endif