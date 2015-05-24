using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Auto Sprite Collider")]
public class D2D_AutoSpriteCollider : D2D_SpriteCollider
{
	[SerializeField]
	private new PolygonCollider2D collider;
	
	public override void RebuildAllColliders()
	{
		UpdateCollidable();
		
		DestroyCollider();
		
		var alphaTex = destructibleSprite.AlphaTex;
		
		if (alphaTex != null)
		{
			var spriteRenderer = D2D_Helper.GetOrAddComponent<SpriteRenderer>(child);
			var sprite         = Sprite.Create(alphaTex, new Rect(0, 0, alphaTex.width, alphaTex.height), Vector2.zero, 1.0f, 0, SpriteMeshType.FullRect);
			
			spriteRenderer.sprite = sprite;
			
			collider = child.AddComponent<PolygonCollider2D>();
			
			// Disable the collider if it couldn't form any triangles
			collider.enabled = IsDefaultPolygonCollider2D(collider) == false;
			
			D2D_Helper.Destroy(sprite);
			D2D_Helper.Destroy(spriteRenderer);
			
			UpdateColliderSettings();
		}
	}
	
	public override void RebuildColliders(int xMin, int xMax, int yMin, int yMax)
	{
		RebuildAllColliders();
	}
	
	public override void UpdateColliderSettings()
	{
		if (collider != null)
		{
			collider.isTrigger      = IsTrigger;
			collider.sharedMaterial = Material;
		}
	}
	
	private void DestroyCollider()
	{
		collider = D2D_Helper.Destroy(collider);
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
}