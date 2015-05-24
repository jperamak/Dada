using UnityEngine;
using UnityEditor;

public static class D2D_Context
{
	private static bool AddSingleComponentValidate<T>(MenuCommand mc)
		where T : Component
	{
		if (mc != null && mc.context != null)
		{
			var component = mc.context as Component;
			
			if (component != null)
			{
				return component.GetComponent<T>() == null;
			}
		}
		
		return false;
	}
	
	private static void AddSingleComponent<T>(MenuCommand mc)
		where T : Component
	{
		if (mc != null && mc.context != null)
		{
			var component = mc.context as Component;
			
			if (component != null && component.GetComponent<T>() == null)
			{
				Undo.AddComponent<T>(component.gameObject);
			}
		}
	}
	
	[UnityEditor.MenuItem("CONTEXT/SpriteRenderer/Make Destructible", true)]
	private static bool MakeDestructibleValidate(MenuCommand mc)
	{
		return AddSingleComponentValidate<D2D_DestructibleSprite>(mc);
	}
	
	[UnityEditor.MenuItem("CONTEXT/SpriteRenderer/Make Destructible", false)]
	private static void MakeDestructible(MenuCommand mc)
	{
		AddSingleComponent<D2D_DestructibleSprite>(mc);
	}
	
	[UnityEditor.MenuItem("CONTEXT/D2D_Destructible/Add Auto Collider", true)]
	private static bool AddAutoColliderValidate(MenuCommand mc)
	{
		return AddSingleComponentValidate<D2D_AutoSpriteCollider>(mc);
	}
	
	[UnityEditor.MenuItem("CONTEXT/D2D_Destructible/Add Auto Collider", false)]
	private static void AddAutoCollider(MenuCommand mc)
	{
		AddSingleComponent<D2D_AutoSpriteCollider>(mc);
	}
	
	[UnityEditor.MenuItem("CONTEXT/D2D_Destructible/Add Polygon Collider", true)]
	private static bool AddPolygonColliderValidate(MenuCommand mc)
	{
		return AddSingleComponentValidate<D2D_PolygonSpriteCollider>(mc);
	}
	
	[UnityEditor.MenuItem("CONTEXT/D2D_Destructible/Add Polygon Collider", false)]
	private static void AddPolygonCollider(MenuCommand mc)
	{
		AddSingleComponent<D2D_PolygonSpriteCollider>(mc);
	}
	
	[UnityEditor.MenuItem("CONTEXT/D2D_Destructible/Add Edge Collider", true)]
	private static bool AddEdgeColliderValidate(MenuCommand mc)
	{
		return AddSingleComponentValidate<D2D_EdgeSpriteCollider>(mc);
	}
	
	[UnityEditor.MenuItem("CONTEXT/D2D_Destructible/Add Edge Collider", false)]
	private static void AddEdgeCollider(MenuCommand mc)
	{
		AddSingleComponent<D2D_EdgeSpriteCollider>(mc);
	}
	
	[UnityEditor.MenuItem("CONTEXT/D2D_Destructible/Add Quad Fracturer", true)]
	private static bool AddQuadFracturerValidate(MenuCommand mc)
	{
		return AddSingleComponentValidate<D2D_QuadFracturer>(mc);
	}
	
	[UnityEditor.MenuItem("CONTEXT/D2D_Destructible/Add Quad Fracturer", false)]
	private static void AddQuadFracturer(MenuCommand mc)
	{
		AddSingleComponent<D2D_QuadFracturer>(mc);
	}
	
	[UnityEditor.MenuItem("CONTEXT/D2D_Destructible/Make Splittable", true)]
	private static bool MakeSplittableValidate(MenuCommand mc)
	{
		return AddSingleComponentValidate<D2D_Splittable>(mc);
	}
	
	[UnityEditor.MenuItem("CONTEXT/D2D_Destructible/Make Splittable", false)]
	private static void MakeSplittable(MenuCommand mc)
	{
		AddSingleComponent<D2D_Splittable>(mc);
	}
	
	[UnityEditor.MenuItem("CONTEXT/D2D_DestructibleSprite/Make Detachable", true)]
	private static bool MakeDetachableValidate(MenuCommand mc)
	{
		return AddSingleComponentValidate<D2D_DetachableSprite>(mc);
	}
	
	[UnityEditor.MenuItem("CONTEXT/D2D_DestructibleSprite/Make Detachable", false)]
	private static void MakeDetachable(MenuCommand mc)
	{
		AddSingleComponent<D2D_DetachableSprite>(mc);
	}
	
	[UnityEditor.MenuItem("CONTEXT/D2D_DestructibleSprite/Make Swappable", true)]
	private static bool MakeSwappableValidate(MenuCommand mc)
	{
		return AddSingleComponentValidate<D2D_SwappableSprite>(mc);
	}
	
	[UnityEditor.MenuItem("CONTEXT/D2D_DestructibleSprite/Make Swappable", false)]
	private static void MakeSwappable(MenuCommand mc)
	{
		AddSingleComponent<D2D_SwappableSprite>(mc);
	}
}