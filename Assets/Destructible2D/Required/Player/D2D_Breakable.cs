using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(D2D_DestructibleSprite))]
[RequireComponent(typeof(Rigidbody2D))]
[AddComponentMenu("Destructible 2D/D2D Breakable")]
public class D2D_Breakable : MonoBehaviour
{
	public bool ChangeColliderType;
	
	public D2D_SpriteColliderType NewColliderType = D2D_SpriteColliderType.AutoPolygon;
	
	public List<D2D_Anchor> Anchors = new List<D2D_Anchor>();
	
	protected virtual void OnAlphaTexModified()
	{
		var destructibleSprite = GetComponent<D2D_DestructibleSprite>();
		var rb2d               = GetComponent<Rigidbody2D>();
		var anchored           = false;
		
		// Find which anchors we're connected to
		foreach (var anchor in Anchors)
		{
			var collider2Ds = Physics2D.OverlapCircleAll(anchor.transform.position, anchor.ScaledRadius);
			
			foreach (var collider2D in collider2Ds)
			{
				if (collider2D.attachedRigidbody == rb2d)
				{
					anchored = true; goto ExitLoops;
				}
			}
		}
		
	ExitLoops:
		
		// Broken off anchors?
		if (anchored == false)
		{
			// Enable physics
			GetComponent<Rigidbody2D>().isKinematic = false;
			
			// Change collider?
			if (ChangeColliderType == true && destructibleSprite.ColliderType != NewColliderType)
			{
				destructibleSprite.ColliderType = NewColliderType;
				
				destructibleSprite.RebuildColliders();
			}
			
			// Now that it's broken, we no longer need this
			D2D_Helper.Destroy(this);
		}
	}
}