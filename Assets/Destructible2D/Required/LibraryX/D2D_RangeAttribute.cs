using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class D2D_RangeAttribute : PropertyAttribute
{
	public float Min;
	
	public float Max;
	
	public D2D_RangeAttribute(float newMin, float newMax)
	{
		Min = newMin;
		Max = newMax;
	}
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(D2D_RangeAttribute))]
public class D2D_RangeDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (D2D_Helper.BaseRectSet == true)
		{
			position.x     = D2D_Helper.BaseRect.x;
			position.width = D2D_Helper.BaseRect.width;
		}
		
		var Attribute = (D2D_RangeAttribute)attribute;
		
		EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
		EditorGUI.Slider(position, property, Attribute.Min, Attribute.Max, label);
	}
}
#endif