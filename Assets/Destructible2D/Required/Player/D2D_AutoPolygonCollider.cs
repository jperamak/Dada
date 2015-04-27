using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Destructible 2D/D2D Auto Polygon Collider")]
public class D2D_AutoPolygonCollider : D2D_Collider
{
	[SerializeField]
	private PolygonCollider2D polygonCollider2D;
	
	public void RebuildCollider(Texture2D alphaTex)
	{
		DestroyPolygonCollider2D();
		
		if (alphaTex != null)
		{
			if (polygonCollider2D == null)
			{
				var spriteRenderer = D2D_Helper.GetOrAddComponent<SpriteRenderer>(gameObject);
				var sprite         = Sprite.Create(alphaTex, new Rect(0, 0, alphaTex.width, alphaTex.height), Vector2.zero, 1.0f, 0, SpriteMeshType.FullRect);
				
				spriteRenderer.sprite = sprite;
				
				polygonCollider2D = gameObject.AddComponent<PolygonCollider2D>();
				
				// Disable the collider if it couldn't form any triangles
				polygonCollider2D.enabled = IsDefaultPolygonCollider2D(polygonCollider2D) == false;
				
				UpdateColliderSettings();
				
				D2D_Helper.Destroy(sprite);
				D2D_Helper.Destroy(spriteRenderer);
			}
		}
	}
	
	public override void UpdateColliderSettings()
	{
		if (polygonCollider2D != null)
		{
			polygonCollider2D.isTrigger      = IsTrigger;
			polygonCollider2D.sharedMaterial = Material;
		}
	}
	
	protected virtual void OnDestroy()
	{
		D2D_Helper.DestroyManaged(DestroyPolygonCollider2D);
	}
	
	protected override void RebuildAll()
	{
		var destructible = D2D_Helper.GetComponentUpwards<D2D_Destructible>(transform);
		
		if (destructible != null)
		{
			RebuildCollider(destructible.AlphaTex);
		}
	}
	
	private void DestroyPolygonCollider2D()
	{
		if (polygonCollider2D != null)
		{
			D2D_Helper.Destroy(polygonCollider2D);
			
			polygonCollider2D = null;
		}
	}
	
	// The default collider is a pentagon, but its position and size changes based on the sprite
	private static bool IsDefaultPolygonCollider2D(PolygonCollider2D polygonCollider2D)
	{
		if (polygonCollider2D == null) return false;
		
		if (polygonCollider2D.GetTotalPointCount() != 5) return false;
		
		var points  = polygonCollider2D.points;
		var spacing = Vector2.Distance(points[0], points[4]);
		
		// Same spacing?
		for (var i = 0; i < 4; i++)
		{
			var spacing2 = Vector2.Distance(points[i], points[i + 1]);
			
			if (Mathf.Approximately(spacing, spacing2) == false)
			{
				return false;
			}
		}
		
		var midpoint = (points[0] + points[1] + points[2] + points[3] + points[4]) * 0.2f;
		var radius   = Vector2.Distance(points[0], midpoint);
		
		// Same radius?
		for (var i = 1; i < 5; i++)
		{
			var radius2 = Vector2.Distance(points[i], midpoint);
			
			if (Mathf.Approximately(radius, radius2) == false)
			{
				return false;
			}
		}
		
		// Must be a pentagon then!
		return true;
	}
	
#if UNITY_EDITOR
	protected override void SetHideFlags(HideFlags hideFlags)
	{
		if (polygonCollider2D != null)
		{
			polygonCollider2D.hideFlags = hideFlags;
		}
	}
#endif
}