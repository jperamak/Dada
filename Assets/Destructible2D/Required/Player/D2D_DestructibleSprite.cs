using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Destructible Sprite")]
public class D2D_DestructibleSprite : D2D_Destructible
{
	public static List<D2D_DestructibleSprite> DestructibleSprites = new List<D2D_DestructibleSprite>();
	
	[D2D_RangeAttribute(1.0f, 100.0f)]
	public float Sharpness = 1.0f;
	
	[SerializeField]
	private SpriteRenderer spriteRenderer;
	
	[SerializeField]
	private float expectedPixelsToUnits;
	
	[SerializeField]
	private Vector2 expectedPivot;
	
	private static Material defaultMaterial;
	
	private static MaterialPropertyBlock propertyBlock;
	
	public Material DefaultMaterial
	{
		get
		{
			if (defaultMaterial == null)
			{
				defaultMaterial = Resources.Load<Material>("Sprites-Default (Destructible 2D)");
			}
			
			return defaultMaterial;
		}
	}
	
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
					var scale  = CalculateAlphaTexScale();
					var offset = CalculateAlphaTexOffset();
					var s      = D2D_Helper.ScalingMatrix(D2D_Helper.Reciprocal(scale));
					var t      = D2D_Helper.TranslationMatrix(-offset);
					
					return s * t * transform.worldToLocalMatrix;
				}
			}
			
			return transform.worldToLocalMatrix;
		}
	}
	
	[ContextMenu("Blur + Halve + Sharpen Alpha Tex")]
	public void CombinedHalveAlphaTex()
	{
		BlurAlphaTex();
		HalveAlphaTex();
		
		Sharpness *= 2;
	}
	
	protected override void ResetAlphaData()
	{
		base.ResetAlphaData();
		
		Sharpness = 1.0f;
	}
	
	protected virtual void Awake()
	{
		if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
		
		var material = spriteRenderer.sharedMaterial;
		
		// Replace Sprites-Default with Sprites-Default (Destructible 2D)?
		if (material != null)
		{
			if (material.HasProperty("_AlphaTex") == false)
			{
				if (material.name == "Sprites-Default")
				{
					material = DefaultMaterial;
				}
			}
		}
		else
		{
			material = DefaultMaterial;
		}
	}
	
	protected override void OnEnable()
	{
		base.OnEnable();
		
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
	}
	
	protected virtual void OnWillRenderObject()
	{
		if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
		
		var sprite = spriteRenderer.sprite;
		
		if (sprite != null)
		{
			if (propertyBlock == null) propertyBlock = new MaterialPropertyBlock();
			
			propertyBlock.Clear();
			
			propertyBlock.SetTexture("_MainTex", sprite.texture);
			propertyBlock.SetTexture("_AlphaTex", AlphaTex);
			propertyBlock.SetVector("_AlphaScale", CalculateAlphaScale(sprite));
			propertyBlock.SetVector("_AlphaOffset", CalculateAlphaOffset(sprite));
			propertyBlock.SetFloat("_Sharpness", Sharpness);
			
			spriteRenderer.SetPropertyBlock(propertyBlock);
		}
	}
	
	private Vector2 CalculateAlphaScale(Sprite sprite)
	{
		var texture     = sprite.texture;
		var textureRect = sprite.textureRect;
		var scaleX      = D2D_Helper.Divide(texture.width , Mathf.Floor(textureRect.width ) + AlphaShiftX) * D2D_Helper.Divide(OriginalWidth , AlphaWidth );
		var scaleY      = D2D_Helper.Divide(texture.height, Mathf.Floor(textureRect.height) + AlphaShiftY) * D2D_Helper.Divide(OriginalHeight, AlphaHeight);
		
		return new Vector2(scaleX, scaleY);
	}
	
	private Vector2 CalculateAlphaOffset(Sprite sprite)
	{
		var scalingX = D2D_Helper.Divide(Mathf.Floor(sprite.textureRect.width ), OriginalWidth );
		var scalingY = D2D_Helper.Divide(Mathf.Floor(sprite.textureRect.height), OriginalHeight);
		
		var texture     = sprite.texture;
		var textureRect = sprite.textureRect;
		var offsetX     = D2D_Helper.Divide(Mathf.Ceil(textureRect.x + AlphaX * scalingX) - AlphaShiftX / 2, texture.width );
		var offsetY     = D2D_Helper.Divide(Mathf.Ceil(textureRect.y + AlphaY * scalingY) - AlphaShiftY / 2, texture.height);
		
		return new Vector2(offsetX, offsetY);
	}
	
	public Vector3 CalculateAlphaTexScale()
	{
		if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
		
		var scale  = Vector3.one;
		var sprite = spriteRenderer.sprite;
		
		if (AlphaTex != null && sprite != null)
		{
			scale.x = D2D_Helper.Divide(sprite.bounds.size.x, sprite.rect.width ) * D2D_Helper.Divide(Mathf.Floor(sprite.textureRect.width ) + AlphaShiftX, AlphaWidth ) * D2D_Helper.Divide(AlphaWidth , OriginalWidth );
			scale.y = D2D_Helper.Divide(sprite.bounds.size.y, sprite.rect.height) * D2D_Helper.Divide(Mathf.Floor(sprite.textureRect.height) + AlphaShiftY, AlphaHeight) * D2D_Helper.Divide(AlphaHeight, OriginalHeight);
		}
		
		return scale;
	}
	
	public Vector3 CalculateAlphaTexOffset()
	{
		if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
		
		var offset = Vector3.one;
		var sprite = spriteRenderer.sprite;
		
		if (AlphaTex != null && sprite != null)
		{
			var scalingX = D2D_Helper.Divide(Mathf.Floor(sprite.textureRect.width ), OriginalWidth );
			var scalingY = D2D_Helper.Divide(Mathf.Floor(sprite.textureRect.height), OriginalHeight);
			
			offset.x = sprite.bounds.min.x + sprite.bounds.size.x * (D2D_Helper.Divide(Mathf.Ceil(sprite.textureRectOffset.x) + AlphaX * scalingX - AlphaShiftX / 2, sprite.rect.width ));
			offset.y = sprite.bounds.min.y + sprite.bounds.size.y * (D2D_Helper.Divide(Mathf.Ceil(sprite.textureRectOffset.y) + AlphaY * scalingY - AlphaShiftY / 2, sprite.rect.height));
		}
		
		return offset;
	}
}