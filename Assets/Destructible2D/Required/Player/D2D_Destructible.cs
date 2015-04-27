using UnityEngine;
using System.Collections.Generic;

public enum D2D_SpriteSplitOrder
{
	Default,
	KeepLargest,
	KeepSmallest
}

public abstract class D2D_Destructible : MonoBehaviour
{
	public static List<D2D_Destructible> Destructibles = new List<D2D_Destructible>();
	
	public static List<D2D_Destructible> DestructiblesCopy = new List<D2D_Destructible>();
	
	public Texture2D DensityTex;
	
	public bool AllowSplit;
	
	public int SplitMinPixels = 20;
	
	[D2D_RangeAttribute(0.0f, 1.0f)]
	public float SplitThreshold = 0.25f;
	
	public D2D_SpriteSplitOrder SplitOrder = D2D_SpriteSplitOrder.Default;
	
	public int OriginalSolidPixelCount;
	
	[System.NonSerialized]
	private Texture2D alphaTex;
	
	[SerializeField]
	private float alphaScaleX;
	
	[SerializeField]
	private float alphaScaleY;
	
	[SerializeField]
	private byte[] alphaTexData;
	
	[SerializeField]
	private int alphaTexWidth;
	
	[SerializeField]
	private int alphaTexHeight;
	
	[SerializeField]
	private int cachedSolidPixelCount = -1;
	
	private bool dirty;
	
	public Texture2D AlphaTex
	{
		get
		{
			DeserializeAlphaTex();
			
			return alphaTex;
		}
	}
	
	public int SolidPixelCount
	{
		get
		{
			if (cachedSolidPixelCount == -1)
			{
				cachedSolidPixelCount = 0;
				
				DeserializeAlphaTex();
				
				if (alphaTex != null && alphaTex.width > 0 && alphaTex.height > 0)
				{
					/*
					for (var y = 0; y < alphaTex.height; y++)
					{
						for (var x = 0; x < alphaTex.width; x++)
						{
							if (alphaTex.GetPixel(x, y).a >= 0.5f)
							{
								cachedSolidPixelCount += 1;
							}
						}
					}
					*/
					// This is faster?
					var pixels = alphaTex.GetPixels32();
					
					for (var i = pixels.Length - 1; i >= 0; i--)
					{
						if (pixels[i].a >= 128)
						{
							cachedSolidPixelCount += 1;
						}
					}
				}
			}
			
			return cachedSolidPixelCount;
		}
	}
	
	public float SolidPixelRatio
	{
		get
		{
			return D2D_Helper.Divide(SolidPixelCount, OriginalSolidPixelCount);
		}
	}
	
	public abstract Matrix4x4 WorldToPixelMatrix
	{
		get;
	}
	
	public void ResizeAlphaTex(int newWidth, int newHeight)
	{
		DeserializeAlphaTex();
		
		if (alphaTex != null)
		{
			ReplaceAlphaWith(new D2D_Pixels(alphaTex).GetResized(newWidth, newHeight));
		}
	}
	
	[ContextMenu("Halve Alpha Tex")]
	public void HalveAlphaTex()
	{
		DeserializeAlphaTex();
		
		if (alphaTex != null)
		{
			ResizeAlphaTex(alphaTex.width / 2, alphaTex.height / 2);
		}
	}
	
	[ContextMenu("Blur Alpha Tex")]
	public void BlurAlphaTex()
	{
		DeserializeAlphaTex();
		
		if (alphaTex != null)
		{
			ReplaceAlphaWith(new D2D_Pixels(alphaTex).GetBlurredAlpha());
		}
	}
	
	[ContextMenu("Recalculate Original Solid Pixel Count")]
	public void RecalculateOriginalSolidPixelCount()
	{
		OriginalSolidPixelCount = SolidPixelCount;
	}
	
	public void ReplaceAlphaWith(Sprite newSprite)
	{
		ReplaceAlphaWith(newSprite != null ? new D2D_Pixels(newSprite) : null);
	}
	
	public void ReplaceAlphaWith(Texture2D newTexture)
	{
		ReplaceAlphaWith(newTexture != null ? new D2D_Pixels(newTexture) : null);
	}
	
	public void ReplaceAlphaWith(int newWidth, int newHeight, Color32[] newPixels)
	{
		ReplaceAlphaWith(new D2D_Pixels(newWidth, newHeight, newPixels));
	}
	
	public void ReplaceAlphaWith(D2D_Pixels newPixels)
	{
#if UNITY_EDITOR
		if (alphaTex != null)
		{
			D2D_Helper.Record(this, "Modify Alpha Tex");
		}
#endif
		if (newPixels != null)
		{
			// Remake texture?
			//if (alphaTex == null || alphaTex.width != newPixels.Width || alphaTex.height != newPixels.Height)
			{
				DestroyAlphaTex();
				
				alphaTexData   = newPixels.ApplyAlpha();
				alphaTexWidth  = newPixels.Width;
				alphaTexHeight = newPixels.Height;
				alphaScaleX    = D2D_Helper.Reciprocal(alphaTexWidth  - 1);
				alphaScaleY    = D2D_Helper.Reciprocal(alphaTexHeight - 1);
			}
			// Copy settings over?
			//else
			{
			//	alphaTex.SetPixels32(newPixels.Pixels);
			//	alphaTex.Apply();
			}
			
			UpdateRect(0, alphaTexWidth, 0, alphaTexHeight);
		}
		else
		{
			DestroyAlphaTex();
			
			UpdateRect(0, 0, 0, 0);
		}
		
		MarkAsDirty();
		
		NotifyChanges();
		
#if UNITY_EDITOR
		D2D_Helper.SetDirty(this);
#endif
	}
	
	public static Matrix4x4 CalculateStampMatrix(Vector2 position, Vector2 size, float angle)
	{
		var t = D2D_Helper.TranslationMatrix(position.x, position.y, 0.0f);
		var r = D2D_Helper.RotationMatrix(Quaternion.Euler(0.0f, 0.0f, angle));
		var s = D2D_Helper.ScalingMatrix(size.x, size.y, 1.0f);
		var o = D2D_Helper.TranslationMatrix(-0.5f, -0.5f, 0.0f); // Centre stamp
		
		return t * r * s * o;
	}
	
	public static void SliceAll(Vector2 startPos, Vector2 endPos, float thickness, Texture2D stampTex, float hardness, int layerMask = -1)
	{
		if (stampTex != null)
		{
			var mid   = (startPos + endPos) / 2.0f;
			var vec   = endPos - startPos;
			var size  = new Vector2(thickness, vec.magnitude);
			var angle = D2D_Helper.Atan2(vec) * -Mathf.Rad2Deg;
			
			StampAll(CalculateStampMatrix(mid, size, angle), stampTex, hardness, layerMask);
		}
	}
	
	public static void StampAll(Vector2 position, Vector2 size, float angle, Texture2D stampTex, float hardness, int layerMask = -1)
	{
		if (stampTex != null)
		{
			StampAll(CalculateStampMatrix(position, size, angle), stampTex, hardness, layerMask);
		}
	}
	
	public static void StampAll(Matrix4x4 matrix, Texture2D stampTex, float hardness, int layerMask = -1)
	{
		if (stampTex != null)
		{
			// The original list may change during the stamping, so make a clone
			DestructiblesCopy.Clear();
			DestructiblesCopy.AddRange(Destructibles);
			
			foreach (var destructible in DestructiblesCopy)
			{
				if (destructible != null)
				{
					var mask = 1 << destructible.gameObject.layer;
					
					if ((layerMask & mask) != 0)
					{
						destructible.Stamp(matrix, stampTex, hardness);
					}
				}
			}
		}
	}
	
	public float GetAlphaAll(Vector2 position, int layerMask = -1)
	{
		var alpha = default(float);
		
		GetAlphaAll(position, ref alpha);
		
		return alpha;
	}
	
	public bool GetAlphaAll(Vector2 position, ref float alpha, int layerMask = -1)
	{
		foreach (var destructible in Destructibles)
		{
			if (destructible != null)
			{
				var mask = 1 << destructible.gameObject.layer;
				
				if ((layerMask & mask) != 0)
				{
					if (destructible.GetAlpha(position, ref alpha) == true)
					{
						return true;
					}
				}
			}
		}
		
		return false;
	}
	
	public float GetAlpha(Vector2 position)
	{
		var alpha = default(float);
		
		GetAlpha(position, ref alpha);
		
		return alpha;
	}
	
	public bool GetAlpha(Vector2 position, ref float alpha)
	{
		if (alphaTexData != null && alphaTexData.Length > 0)
		{
			var point = WorldToPixelMatrix.MultiplyPoint(position);
			var x     = Mathf.FloorToInt(point.x);
			var y     = Mathf.FloorToInt(point.y);
			
			if (x >= 0 && x < alphaTexWidth)
			{
				if (y >= 0 && y < alphaTexHeight)
				{
					alpha = alphaTexData[x + y * alphaTexWidth]; return true;
				}
			}
		}
		
		return false;
	}
	
	public void Stamp(Vector2 position, Vector2 size, float angle, Texture2D stampTex, float hardness)
	{
		DeserializeAlphaTex();
		
		if (alphaTex != null && stampTex != null)
		{
			Stamp(CalculateStampMatrix(position, size, angle), stampTex, hardness);
		}
	}
	
	public void Stamp(Matrix4x4 stampMatrix, Texture2D stampTex, float hardness)
	{
		if (alphaTex != null && stampTex != null)
		{
			var stampToPixelMatrix = WorldToPixelMatrix * stampMatrix;
			var pixelToStampMatrix = stampToPixelMatrix.inverse;
#if UNITY_EDITOR
			D2D_Helper.MakeTextureReadable(stampTex);
			D2D_Helper.MakeTextureReadable(DensityTex);
#endif
			// Project corners of stamp
			// TODO: account for non-orthogonal matrices?
			var bl = stampToPixelMatrix.MultiplyPoint(new Vector3(0.0f, 0.0f, 0.0f));
			var br = stampToPixelMatrix.MultiplyPoint(new Vector3(1.0f, 0.0f, 0.0f));
			var tl = stampToPixelMatrix.MultiplyPoint(new Vector3(0.0f, 1.0f, 0.0f));
			var tr = stampToPixelMatrix.MultiplyPoint(new Vector3(1.0f, 1.0f, 0.0f));
			
			// Find AABB of stamp
			var xMin = Mathf.FloorToInt(Mathf.Min(Mathf.Min(bl.x, br.x), Mathf.Min(tl.x, tr.x)));
			var xMax = Mathf.FloorToInt(Mathf.Max(Mathf.Max(bl.x, br.x), Mathf.Max(tl.x, tr.x)));
			var yMin = Mathf.FloorToInt(Mathf.Min(Mathf.Min(bl.y, br.y), Mathf.Min(tl.y, tr.y)));
			var yMax = Mathf.FloorToInt(Mathf.Max(Mathf.Max(bl.y, br.y), Mathf.Max(tl.y, tr.y)));
			
			xMin = Mathf.Clamp(xMin, 0, alphaTexWidth  - 1);
			xMax = Mathf.Clamp(xMax, 0, alphaTexWidth  - 1);
			yMin = Mathf.Clamp(yMin, 0, alphaTexHeight - 1);
			yMax = Mathf.Clamp(yMax, 0, alphaTexHeight - 1);
			
			if (xMax > xMin && yMax > yMin)
			{
				MarkAsDirty();
				
				for (var y = yMin; y <= yMax; y++)
				{
					for (var x = xMin; x <= xMax; x++)
					{
						var uv = pixelToStampMatrix.MultiplyPoint(new Vector3(x, y, 0.0f));
						
						// Is this pixel within the stamp?
						if (uv.x >= 0.0f && uv.y >= 0.0f && uv.x < 1.0f && uv.y < 1.0f)
						{
							var stamp = stampTex.GetPixel(Mathf.FloorToInt(uv.x * stampTex.width), Mathf.FloorToInt(uv.y * stampTex.height));
							
							FastPaint(x, y, stamp.a * hardness);
						}
					}
				}
				
				if (alphaTex != null)
				{
					alphaTex.Apply();
				}
				
				var split = false;
				
				if (AllowSplit == true)
				{
					split = D2D_SplitCalculator.Generate(this, SplitOrder);
				}
				
				if (split == false)
				{
					UpdateRect(xMin, xMax, yMin, yMax);
					
					NotifyChanges();
				}
			}
		}
	}
	
	private void FastPaint(int x, int y, float opacity)
	{
		var index = x + y * alphaTexWidth;
		var alpha = alphaTexData[index];
		
		if (DensityTex != null)
		{
			var u       = x * alphaScaleX * DensityTex.width;
			var v       = y * alphaScaleY * DensityTex.height;
			var density = DensityTex.GetPixel(Mathf.FloorToInt(u), Mathf.FloorToInt(v));
			
			opacity *= 1.0f - density.a;
		}
		
		var a = (int)(opacity * 255.0f);
		
		alpha = (byte)Mathf.Clamp((int)alpha - a, 0, 255);
		
		alphaTexData[index] = alpha;
	}
	
	protected virtual void OnEnable()
	{
		Destructibles.Add(this);
		
#if UNITY_EDITOR
		if (UnityEditor.AssetDatabase.Contains(this) == true)
		{
			return;
		}
#endif
		CacheTexures();
	}
	
	protected virtual void OnDisable()
	{
		Destructibles.Remove(this);
	}
	
	protected virtual void OnDestroy()
	{
		DestroyAlphaTex();
	}
	
#if UNITY_EDITOR
	protected virtual void OnValidate()
	{
		D2D_Helper.MakeTextureReadable(DensityTex);
	}
#endif
	
	protected abstract void UpdateRect(int xMin, int xMax, int yMin, int yMax);
	
	private void DeserializeAlphaTex()
	{
		if ((dirty == true || alphaTex == null) && alphaTexData != null && alphaTexData.Length > 0 && alphaTexWidth > 0 && alphaTexHeight > 0)
		{
			dirty = false;
			
			var pixels = new Color32[alphaTexData.Length];
			
			for (var i = pixels.Length - 1; i >= 0; i--)
			{
				pixels[i].a = alphaTexData[i];
			}
			
			alphaTex = new Texture2D(alphaTexWidth, alphaTexHeight, TextureFormat.Alpha8, false);
			alphaTex.hideFlags = HideFlags.DontSave;
			alphaTex.wrapMode  = TextureWrapMode.Clamp;
			alphaTex.SetPixels32(pixels);
			alphaTex.Apply();
		}
	}
	
	private void DestroyAlphaTex()
	{
		if (alphaTex != null)
		{
			D2D_Helper.Destroy(alphaTex);
			
			alphaTex = null;
		}
		
		alphaTexData   = null;
		alphaTexWidth  = 0;
		alphaTexHeight = 0;
	}
	
	private void CacheTexures()
	{
		if (DensityTex != null)
		{
			if (DensityTex.width > 0 && DensityTex.height > 0)
			{
				DensityTex.GetPixel(0, 0);
			}
		}
	}
	
	private void MarkAsDirty()
	{
		dirty                 = true;
		cachedSolidPixelCount = -1;
	}
	
	private void NotifyChanges()
	{
#if UNITY_EDITOR
		if (Application.isPlaying == false)
		{
			return;
		}
#endif
		BroadcastMessage("OnAlphaTexModified", SendMessageOptions.DontRequireReceiver);
	}
}