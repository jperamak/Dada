using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(D2D_RuntimeSprite))]
public class D2D_RuntimeSprite_Editor : D2D_Editor<D2D_RuntimeSprite>
{
	protected override void OnInspector()
	{
		EditorGUILayout.HelpBox("This component will clean up the runtime generated Sprite used to make this GameObject.", MessageType.Info);
	}
}