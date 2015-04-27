using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(D2D_Breakable))]
public class D2D_Breakable_Editor : D2D_Editor<D2D_Breakable>
{
	protected override void OnInspector()
	{
		if (Any(t => t.GetComponent<D2D_DestructibleSprite>().AllowSplit == false))
		{
			if (HelpButton("Breakable sprites require Allow Split", MessageType.Error, "Set Allow Split", 50.0f) == true)
			{
				Each(t => t.GetComponent<D2D_DestructibleSprite>().AllowSplit = true);
			}
		}
		
		if (Any(t => t.GetComponent<Rigidbody2D>().isKinematic == false))
		{
			if (HelpButton("Breakable sprites require Is Kinematic", MessageType.Error, "Set Is Kinematic", 50.0f) == true)
			{
				Each(t => t.GetComponent<Rigidbody2D>().isKinematic = true);
			}
		}
		
		DrawDefault("ChangeColliderType");
		
		if (Any(t => t.ChangeColliderType == true))
		{
			DrawDefault("NewColliderType");
			
			if (Any(t => t.NewColliderType == D2D_SpriteColliderType.Edge))
			{
				if (HelpButton("Dynamic edge colliders may not work well", MessageType.Warning, "Change", 50.0f) == true)
				{
					Each(t => {if (t.NewColliderType == D2D_SpriteColliderType.Edge) t.NewColliderType = D2D_SpriteColliderType.AutoPolygon; });
				}
			}
			
			if (Any(t => t.NewColliderType == t.GetComponent<D2D_DestructibleSprite>().ColliderType))
			{
				EditorGUILayout.HelpBox("This is the same Collider Type as the source.", MessageType.Warning);
			}
		}
		
		BeginError(Any(t => t.Anchors.Count == 0));
		{
			DrawDefault("Anchors");
		}
		EndError();
	}
}