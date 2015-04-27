using UnityEngine;
using System.Collections.Generic;

public enum D2D_SpriteColliderType
{
	None,
	Edge,
	Polygon,
	AutoPolygon
}

[ExecuteInEditMode]
[AddComponentMenu("Destructible 2D/D2D Destructible Sprite")]
[RequireComponent(typeof(SpriteRenderer))]
public class D2D_DestructibleSprite : D2D_Destructible
{
	public static List<D2D_DestructibleSprite> DestructibleSprites = new List<D2D_DestructibleSprite>();
	
	public Material SourceMaterial;
	
	[D2D_RangeAttribute(1.0f, 10.0f)]
	public float Sharpness = 1.0f;
	
	public D2D_SpriteColliderType ColliderType = D2D_SpriteColliderType.None;
	
	[SerializeField]
	private SpriteRenderer spriteRenderer;
	
	[SerializeField]
	private D2D_EdgeColliders edgeColliders;
	
	[SerializeField]
	private D2D_PolygonColliders polygonColliders;
	
	[SerializeField]
	private D2D_AutoPolygonCollider autoPolygonCollider;
	
	[SerializeField]
	private float expectedPixelsToUnits;
	
	[SerializeField]
	private Vector2 expectedPivot;
	
	[SerializeField]
	private Material clonedMaterial;
	
	public override Matrix4x4 WorldToPixelMatrix
	{
		get
		{
			if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
			
			if (AlphaTex != null)
			{
				var sprite = spriteRenderer.sprite;
				
				if (sprite != null)
				{
					var scale  = CalculateAlphaTexScale(sprite);
					var offset = CalculateAlphaTexOffset(sprite);
					var s      = D2D_Helper.ScalingMatrix(D2D_Helper.Reciprocal(scale));
					var t      = D2D_Helper.TranslationMatrix(-offset);
					
					return s * t * transform.worldToLocalMatrix;
				}
			}
			
			return transform.worldToLocalMatrix;
		}
	}
	
	[ContextMenu("Halve Alpha Tex And Split Min Pixels")]
	public void HalveAlphaTexAndSplitMinPixels()
	{
		if (AlphaTex != null)
		{
			HalveAlphaTex();
			
			SplitMinPixels = Mathf.Max(1, SplitMinPixels / 2);
		}
	}
	
	[ContextMenu("Blur Alpha Tex And Double Sharpness")]
	public void BlurAlphaTexAndDoubleSharpness()
	{
		if (AlphaTex != null)
		{
			BlurAlphaTex();
			
			Sharpness *= 2;
		}
	}
	
	public void RebuildColliders()
	{
		if (AlphaTex != null && AlphaTex.width > 0 && AlphaTex.height > 0)
		{
			RebuildColliders(0, AlphaTex.width, 0, AlphaTex.height);
		}
	}
	
	protected override void OnEnable()
	{
		base.OnEnable();
		
		// Has this been cloned?
		if (clonedMaterial != null)
		{
			foreach (var destructibleSprite in DestructibleSprites)
			{
				if (destructibleSprite != null && destructibleSprite.clonedMaterial == clonedMaterial)
				{
					OnDuplicate();
				}
			}
		}
		
		DestructibleSprites.Add(this);
	}
	
	protected override void OnDisable()
	{
		base.OnDisable();
		
		DestructibleSprites.Remove(this);
	}
	
	protected virtual void Update()
	{
		if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
		
		// Copy alpha from main tex
		if (AlphaTex == null && spriteRenderer.sprite != null)
		{
			ReplaceAlphaWith(spriteRenderer.sprite);
			
			RecalculateOriginalSolidPixelCount();
		}
		
		#if UNITY_EDITOR
		D2D_Helper.MakeTextureReadable(DensityTex);
		#endif
		
		UpdateColliders();
	}
	
	protected override void OnDestroy()
	{
		base.OnDestroy();
		
		DestroyAutoPolygonCollider();
		DestroyPolygonColliders();
		DestroyEdgeColliders();
		DestroyMaterial();
	}
	
	protected virtual void OnWillRenderObject()
	{
		if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
		
		UpdateSourceMaterial();
		
		DestroyMaterialIfSettingsDiffer();
		
		var sprite = spriteRenderer.sprite;
		
		if (SourceMaterial != null && sprite != null)
		{
			// Clone new material?
			if (clonedMaterial == null)
			{
				clonedMaterial = D2D_Helper.Clone(SourceMaterial, false);
			}
			else
			{
				clonedMaterial.CopyPropertiesFromMaterial(SourceMaterial);
			}
			
			#if UNITY_EDITOR
			clonedMaterial.hideFlags = HideFlags.DontSave;
			#endif
			
			clonedMaterial.SetTexture("_MainTex", sprite.texture);
			clonedMaterial.SetTexture("_AlphaTex", AlphaTex);
			clonedMaterial.SetVector("_AlphaScale", CalculateAlphaScale(sprite));
			clonedMaterial.SetVector("_AlphaOffset", CalculateAlphaOffset(sprite));
			clonedMaterial.SetFloat("_Sharpness", Sharpness);
			
			#if UNITY_EDITOR
			clonedMaterial.hideFlags = HideFlags.None;
			#endif
		}
		
		if (spriteRenderer.sharedMaterial != clonedMaterial)
		{
			spriteRenderer.sharedMaterial = clonedMaterial;
		}
	}
	
	protected override void UpdateRect(int xMin, int xMax, int yMin, int yMax)
	{
		RebuildColliders(xMin, xMax, yMin, yMax);
	}
	
	protected virtual void OnDuplicate()
	{
		if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
		
		if (clonedMaterial == spriteRenderer.sharedMaterial)
		{
			clonedMaterial = D2D_Helper.Clone(clonedMaterial);
			
			spriteRenderer.sharedMaterial = clonedMaterial;
		}
	}
	
	private Vector2 CalculateAlphaScale(Sprite sprite)
	{
		var texture     = sprite.texture;
		var textureRect = sprite.textureRect;
		var scaleX      = texture.width / textureRect.width;
		var scaleY      = texture.height / textureRect.height;
		
		return new Vector2(scaleX, scaleY);
	}
	
	private Vector2 CalculateAlphaOffset(Sprite sprite)
	{
		var texture     = sprite.texture;
		var textureRect = sprite.textureRect;
		var offsetX     = textureRect.x / texture.width;
		var offsetY     = textureRect.y / texture.height;
		
		return new Vector2(offsetX, offsetY);
	}
	
	private void UpdateSourceMaterial()
	{
		// Do we need to set a source material?
		if (SourceMaterial == null)
		{
			if (spriteRenderer.sharedMaterial != null)
			{
				SourceMaterial = spriteRenderer.sharedMaterial;
			}
			else
			{
				SourceMaterial = Resources.Load<Material>("Sprites-Default (Destructible 2D)");
			}
		}
		
		// Replace Sprites-Default with Sprites-Default (Destructible 2D)?
		if (SourceMaterial != null && SourceMaterial.HasProperty("_AlphaTex") == false)
		{
			if (SourceMaterial.name == "Sprites-Default")
			{
				SourceMaterial = Resources.Load<Material>("Sprites-Default (Destructible 2D)");
			}
		}
	}
	
	private void RebuildColliders(int xMin, int xMax, int yMin, int yMax)
	{
		switch (ColliderType)
		{
		case D2D_SpriteColliderType.Edge:
		{
			if (edgeColliders != null)
			{
				edgeColliders.RebuildColliders(AlphaTex, xMin, xMax, yMin, yMax);
			}
		}
			break;
			
		case D2D_SpriteColliderType.Polygon:
		{
			if (polygonColliders != null)
			{
				polygonColliders.RebuildColliders(AlphaTex, xMin, xMax, yMin, yMax);
			}
		}
			break;
			
		case D2D_SpriteColliderType.AutoPolygon:
		{
			if (autoPolygonCollider != null)
			{
				autoPolygonCollider.RebuildCollider(AlphaTex);
			}
		}
			break;
		}
	}
	
	private void UpdateColliders()
	{
		if (ColliderType != D2D_SpriteColliderType.None)
		{
			var colliderTransform = default(Transform);
			
			switch (ColliderType)
			{
			case D2D_SpriteColliderType.Edge:
			{
				DestroyAutoPolygonCollider();
				DestroyPolygonColliders();
				
				if (edgeColliders == null)
				{
					edgeColliders = D2D_Helper.CreateGameObject("Edge Colliders", transform).AddComponent<D2D_EdgeColliders>();
					edgeColliders.RebuildAllColliders(AlphaTex);
				}
				
				colliderTransform = edgeColliders.transform;
			}
				break;
				
			case D2D_SpriteColliderType.Polygon:
			{
				DestroyAutoPolygonCollider();
				DestroyEdgeColliders();
				
				if (polygonColliders == null)
				{
					polygonColliders = D2D_Helper.CreateGameObject("Polygon Colliders", transform).AddComponent<D2D_PolygonColliders>();
					polygonColliders.RebuildAllColliders(AlphaTex);
				}
				
				colliderTransform = polygonColliders.transform;
			}
				break;
				
			case D2D_SpriteColliderType.AutoPolygon:
			{
				DestroyPolygonColliders();
				DestroyEdgeColliders();
				
				if (autoPolygonCollider == null)
				{
					autoPolygonCollider = D2D_Helper.CreateGameObject("Auto Polygon Collider", transform).AddComponent<D2D_AutoPolygonCollider>();
					autoPolygonCollider.RebuildCollider(AlphaTex);
				}
				
				colliderTransform = autoPolygonCollider.transform;
			}
				break;
			}
			
			if (colliderTransform != null)
			{
				var colliderScale  = Vector3.one;
				var colliderOffset = Vector3.zero;
				
				if (spriteRenderer != null && AlphaTex != null)
				{
					var sprite = spriteRenderer.sprite;
					
					// Magic to align the colliders with the sprite renderer
					if (sprite != null)
					{
						colliderScale  = CalculateAlphaTexScale(sprite);
						colliderOffset = CalculateAlphaTexOffset(sprite);
					}
				}
				
				if (colliderTransform.localPosition != colliderOffset)
				{
					colliderTransform.localPosition = colliderOffset;
				}
				
				if (colliderTransform.localScale != colliderScale)
				{
					colliderTransform.localScale = colliderScale;
				}
			}
		}
		else
		{
			DestroyAutoPolygonCollider();
			DestroyPolygonColliders();
			DestroyEdgeColliders();
		}
	}
	
	private Vector3 CalculateAlphaTexScale(Sprite sprite)
	{
		var scale = Vector3.one;
		
		scale.x = D2D_Helper.Divide(sprite.bounds.size.x, sprite.rect.width) * D2D_Helper.Divide(sprite.textureRect.width, AlphaTex.width);
		scale.y = D2D_Helper.Divide(sprite.bounds.size.y, sprite.rect.height) * D2D_Helper.Divide(sprite.textureRect.height, AlphaTex.height);
		
		return scale;
	}
	
	private Vector3 CalculateAlphaTexOffset(Sprite sprite)
	{
		var offset = Vector3.one;
		
		offset.x = sprite.bounds.min.x + sprite.bounds.size.x * D2D_Helper.Divide(sprite.textureRectOffset.x, sprite.rect.width);
		offset.y = sprite.bounds.min.y + sprite.bounds.size.y * D2D_Helper.Divide(sprite.textureRectOffset.y, sprite.rect.height);
		
		return offset;
	}
	
	private void DestroyMaterialIfSettingsDiffer()
	{
		if (clonedMaterial != null)
		{
			if (SourceMaterial == null)
			{
				DestroyMaterial(); return;
			}
			
			if (clonedMaterial.shader != SourceMaterial.shader)
			{
				DestroyMaterial(); return;
			}
		}
	}
	
	private void DestroyEdgeColliders()
	{
		if (edgeColliders != null)
		{
			D2D_Helper.Destroy(edgeColliders.gameObject);
			
			edgeColliders = null;
		}
	}
	
	private void DestroyPolygonColliders()
	{
		if (polygonColliders != null)
		{
			D2D_Helper.Destroy(polygonColliders.gameObject);
			
			polygonColliders = null;
		}
	}
	
	private void DestroyAutoPolygonCollider()
	{
		if (autoPolygonCollider != null)
		{
			D2D_Helper.Destroy(autoPolygonCollider.gameObject);
			
			autoPolygonCollider = null;
		}
	}
	
	private void DestroyMaterial()
	{
		D2D_Helper.Destroy(clonedMaterial);
		
		clonedMaterial = null;
	}
	
	#if UNITY_EDITOR
	[UnityEditor.MenuItem("CONTEXT/SpriteRenderer/Make Destructible", true)]
	private static bool MakeDestructibleValidate(UnityEditor.MenuCommand mc)
	{
		if (mc != null && mc.context != null)
		{
			var spriteRenderer = mc.context as SpriteRenderer;
			
			if (spriteRenderer != null)
			{
				return spriteRenderer.GetComponent<D2D_DestructibleSprite>() == null;
			}
		}
		
		return false;
	}
	
	[UnityEditor.MenuItem("CONTEXT/SpriteRenderer/Make Destructible", false)]
	private static void MakeDestructible(UnityEditor.MenuCommand mc)
	{
		if (mc != null && mc.context != null)
		{
			var spriteRenderer = mc.context as SpriteRenderer;
			
			if (spriteRenderer != null && spriteRenderer.GetComponent<D2D_DestructibleSprite>() == null)
			{
				UnityEditor.Undo.AddComponent<D2D_DestructibleSprite>(spriteRenderer.gameObject);
			}
		}
	}
	#endif
}