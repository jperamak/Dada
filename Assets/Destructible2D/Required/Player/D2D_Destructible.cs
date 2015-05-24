using UnityEngine;
using System.Collections.Generic;

public abstract class D2D_Destructible : MonoBehaviour
{
	public static List<D2D_Destructible> Destructibles = new List<D2D_Destructible>();
	
	public static List<D2D_Destructible> DestructiblesCopy = new List<D2D_Destructible>();
	
	public Texture2D DensityTex;
	
	public bool Indestructible;
	
	public bool Binary;
	
	public int SplitDepth;
	
	public int MinSplitPixels = 50;
	
	[SerializeField]
	private int originalSolidPixelCount;
	
	[SerializeField]
	private int originalWidth;
	
	[SerializeField]
	private int originalHeight;
	
	[SerializeField]
	private int alphaX;
	
	[SerializeField]
	private int alphaY;
	
	[SerializeField]
	private int alphaWidth;
	
	[SerializeField]
	private int alphaHeight;
	
	[SerializeField]
	private byte[] alphaData;
	
	[System.NonSerialized]
	private Texture2D alphaTex;
	
	[SerializeField]
	private float alphaScaleX; // Used for density tex sampling
	
	[SerializeField]
	private float alphaScaleY; // Used for density tex sampling
	
	[SerializeField]
	private float alphaShiftX; // Used to fix odd to even downscaling
	
	[SerializeField]
	private float alphaShiftY; // Used to fix odd to even downscaling
	
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
				
				if (alphaData != null)
				{
					for (var i = alphaData.Length - 1; i >= 0; i--)
					{
						if (alphaData[i] >= 128)
						{
							cachedSolidPixelCount += 1;
						}
					}
				}
			}
			
			return cachedSolidPixelCount;
		}
	}
	
	public int OriginalSolidPixelCount
	{
		get
		{
			return originalSolidPixelCount;
		}
	}
	
	public int OriginalWidth
	{
		get
		{
			return originalWidth;
		}
	}
	
	public int OriginalHeight
	{
		get
		{
			return originalHeight;
		}
	}
	
	public int AlphaX
	{
		get
		{
			return alphaX;
		}
	}
	
	public int AlphaY
	{
		get
		{
			return alphaY;
		}
	}
	
	public int AlphaWidth
	{
		get
		{
			return alphaWidth;
		}
	}
	
	public int AlphaHeight
	{
		get
		{
			return alphaHeight;
		}
	}
	
	public float AlphaShiftX
	{
		get
		{
			return alphaShiftX;
		}
	}
	
	public float AlphaShiftY
	{
		get
		{
			return alphaShiftY;
		}
	}
	
	public byte[] AlphaData
	{
		// NOTE: Don't call this unless you know what you're doing!
		set
		{
			alphaData = value;
		}
		
		get
		{
			return alphaData;
		}
	}
	
	public float SolidPixelRatio
	{
		get
		{
			return D2D_Helper.Divide(SolidPixelCount, originalSolidPixelCount);
		}
	}
	
	public abstract Matrix4x4 WorldToPixelMatrix
	{
		get;
	}
	
	public void MarkAsDirty()
	{
#if UNITY_EDITOR
		if (dirty == false)
		{
			D2D_Helper.SetDirty(this);
		}
#endif
		dirty                 = true;
		cachedSolidPixelCount = -1;
	}
	
	[ContextMenu("Recalculate Original Solid Pixel Count")]
	public void RecalculateOriginalSolidPixelCount()
	{
		originalSolidPixelCount = SolidPixelCount;
	}
	
	[ContextMenu("Blur Alpha Tex")]
	public void BlurAlphaTex()
	{
		D2D_AlphaTex.Load(alphaData, alphaWidth, alphaHeight);
		
		D2D_AlphaTex.Blur();
		
		UpdateAlphaWith(D2D_AlphaTex.Data, D2D_AlphaTex.Width, D2D_AlphaTex.Height);
	}
	
	//[ContextMenu("Halve Alpha Tex")]
	public void HalveAlphaTex()
	{
		D2D_AlphaTex.Load(alphaData, alphaWidth, alphaHeight);
		
		var oldWidth  = alphaWidth;
		var oldHeight = alphaHeight;
		
		originalWidth  /= 2;
		originalHeight /= 2;
		alphaWidth     /= 2;
		alphaHeight    /= 2;
		alphaX         /= 2;
		alphaY         /= 2;
		alphaShiftX    *= 2;
		alphaShiftY    *= 2;
		
		var shiftX = oldWidth  - alphaWidth  * 2;
		var shiftY = oldHeight - alphaHeight * 2;
		
		//D2D_AlphaTex.Halve();
		D2D_AlphaTex.Resize(alphaWidth, alphaHeight);
		
		UpdateAlphaWith(D2D_AlphaTex.Data, D2D_AlphaTex.Width, D2D_AlphaTex.Height);
		
		alphaShiftX += shiftX;
		alphaShiftY += shiftY;
	}
	
	[ContextMenu("Add Fixture")]
	public D2D_Fixture AddFixture()
	{
#if UNITY_EDITOR
		D2D_Helper.BeginUndo("Add Fixture");
#endif
		var fixture = D2D_Helper.CreateGameObject("Fixture", transform, true).AddComponent<D2D_Fixture>();
#if UNITY_EDITOR
		D2D_Helper.SelectAndPing(fixture);
#endif
		return fixture;
	}
	
	public void ReplaceAlphaWith(Sprite newSprite)
	{
		var data   = default(byte[]);
		var width  = default(int);
		var height = default(int);
		
		if (D2D_Helper.ExtractAlphaData(newSprite, ref data, ref width, ref height) == true)
		{
			ReplaceAlphaWith(data, width, height);
		}
		else
		{
			DestroyAlphaTex();
		}
	}
	
	public void ReplaceAlphaWith(Texture2D newTexture)
	{
		if (newTexture != null)
		{
			ReplaceAlphaWith(D2D_Helper.ExtractAlphaData(newTexture), newTexture.width, newTexture.height);
		}
		else
		{
			DestroyAlphaTex();
		}
	}
	
	public void ReplaceAlphaWith(byte[] newAlphaData, int newAlphaWidth, int newAlphaHeight)
	{
		if (newAlphaData != null && newAlphaWidth > 0 && newAlphaHeight > 0 && newAlphaData.Length >= newAlphaWidth * newAlphaHeight)
		{
			ReplaceAlphaData(newAlphaData, newAlphaWidth, newAlphaHeight);
			
			ResetAlphaData();
			
			originalWidth  = newAlphaWidth;
			originalHeight = newAlphaHeight;
			
			MarkAsDirty();
			NotifyChanges();
		}
		else
		{
			DestroyAlphaTex();
		}
	}
	
	public void SubsetAlphaWith(byte[] newAlphaData, int newAlphaWidth, int newAlphaHeight, int offsetX, int offsetY)
	{
		if (newAlphaData != null && newAlphaWidth > 0 && newAlphaHeight > 0 && newAlphaData.Length >= newAlphaWidth * newAlphaHeight)
		{
			ReplaceAlphaData(newAlphaData, newAlphaWidth, newAlphaHeight);
			
			alphaX += offsetX;
			alphaY += offsetY;
			
			MarkAsDirty();
			NotifyChanges();
		}
	}
	
	// This will preserve any subset settings
	public void UpdateAlphaWith(byte[] newAlphaData, int newAlphaWidth, int newAlphaHeight)
	{
		if (alphaWidth == newAlphaWidth && alphaHeight == newAlphaHeight)
		{
			ReplaceAlphaData(newAlphaData, newAlphaWidth, newAlphaHeight);
			
			MarkAsDirty();
			NotifyChanges();
		}
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
				if (destructible != null && destructible.Indestructible == false)
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
	
	public byte GetAlphaAll(Vector2 position, int layerMask = -1)
	{
		var alpha = default(byte);
		
		GetAlphaAll(position, ref alpha);
		
		return alpha;
	}
	
	public bool GetAlphaAll(Vector2 position, ref byte alpha, int layerMask = -1)
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
	
	public byte GetAlpha(Vector2 position)
	{
		var alpha = default(byte);
		
		GetAlpha(position, ref alpha);
		
		return alpha;
	}
	
	public bool GetAlpha(Vector2 position, ref byte alpha)
	{
		if (alphaData != null)
		{
			var point = WorldToPixelMatrix.MultiplyPoint(position);
			var x     = Mathf.FloorToInt(point.x);
			var y     = Mathf.FloorToInt(point.y);
			
			if (x >= 0 && x < alphaWidth)
			{
				if (y >= 0 && y < alphaHeight)
				{
					alpha = alphaData[x + y * alphaWidth]; return true;
				}
			}
		}
		
		return false;
	}
	
	public byte FastGetAlpha(int x, int y)
	{
		return alphaData[x + y * alphaWidth];
	}
	
	public void FastSetAlpha(int x, int y, byte alpha)
	{
		alphaData[x + y * alphaWidth] = alpha;
	}
	
	public void Stamp(Vector2 position, Vector2 size, float angle, Texture2D stampTex, float hardness)
	{
		if (stampTex != null && size != Vector2.zero)
		{
			Stamp(CalculateStampMatrix(position, size, angle), stampTex, hardness);
		}
	}
	
	public void Stamp(Matrix4x4 stampMatrix, Texture2D stampTex, float hardness)
	{
		if (alphaData != null && stampTex != null)
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
			
			xMin = Mathf.Clamp(xMin, 0, alphaWidth  - 1);
			xMax = Mathf.Clamp(xMax, 0, alphaWidth  - 1);
			yMin = Mathf.Clamp(yMin, 0, alphaHeight - 1);
			yMax = Mathf.Clamp(yMax, 0, alphaHeight - 1);
			
			// Is the size greater than 1?
			if (xMax > xMin && yMax > yMin)
			{
				// Make sure the texture is up to date
				DeserializeAlphaTex();
				
				// Only dirty the solid pixel count
				cachedSolidPixelCount = -1;
				
				// Write alpha tex and alpha data?
				if (alphaTex != null)
				{
					for (var y = yMin; y <= yMax; y++)
					{
						for (var x = xMin; x <= xMax; x++)
						{
							var uv = pixelToStampMatrix.MultiplyPoint(new Vector3(x, y, 0.0f));
							
							if (uv.x >= 0.0f && uv.y >= 0.0f && uv.x < 1.0f && uv.y < 1.0f) // Is this pixel within the alpha?
							{
								var stamp = stampTex.GetPixel(Mathf.FloorToInt(uv.x * stampTex.width), Mathf.FloorToInt(uv.y * stampTex.height));
								
								FastPaintBoth(x + y * alphaWidth, x, y, stamp.a * hardness);
							}
						}
					}
					
					alphaTex.Apply();
				}
				// Write just alpha data?
				else
				{
					// The texture needs to be rebuilt, so mark all as dirty
					MarkAsDirty();
					
					for (var y = yMin; y <= yMax; y++)
					{
						for (var x = xMin; x <= xMax; x++)
						{
							var uv = pixelToStampMatrix.MultiplyPoint(new Vector3(x, y, 0.0f));
							
							if (uv.x >= 0.0f && uv.y >= 0.0f && uv.x < 1.0f && uv.y < 1.0f) // Is this pixel within the alpha?
							{
								var stamp = stampTex.GetPixel(Mathf.FloorToInt(uv.x * stampTex.width), Mathf.FloorToInt(uv.y * stampTex.height));
								
								FastPaint(x + y * alphaWidth, x, y, stamp.a * hardness);
							}
						}
					}
				}
				
				NotifyChanges(xMin, xMax, yMin, yMax);
			}
		}
	}
	
	protected virtual void OnDestructibleSplit(D2D_SplitData splitData)
	{
		// Only count this as a valid split if there is more than one child over the split min pixels, or this is smaller
		if (splitData.SolidPixelCounts[splitData.Index] < MinSplitPixels || splitData.SolidPixelThresholdTotal(MinSplitPixels) >= 2)
		{
			SplitDepth += 1;
			
			D2D_Helper.BroadcastMessage(transform, "OnDestructibleValidSplit", splitData, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	protected virtual void ResetAlphaData()
	{
		SplitDepth  = 0;
		alphaX      = 0;
		alphaY      = 0;
		alphaShiftX = 0;
		alphaShiftY = 0;
	}
	
	// Doesn't reallocate memory if the new alpha data is the same size
	private void ReplaceAlphaData(byte[] newAlphaData, int newAlphaWidth, int newAlphaHeight)
	{
		var newAlphaTotal = newAlphaWidth * newAlphaHeight;
		
		if (alphaData == null || alphaData.Length != newAlphaTotal)
		{
			alphaData = new byte[newAlphaTotal];
		}
		
		for (var i = 0; i < newAlphaTotal; i++)
		{
			alphaData[i] = newAlphaData[i];
		}
		
		alphaWidth  = newAlphaWidth;
		alphaHeight = newAlphaHeight;
		alphaScaleX = D2D_Helper.Reciprocal(originalWidth);
		alphaScaleY = D2D_Helper.Reciprocal(originalHeight);
	}
	
	private byte FastPaint(int i, int x, int y, float opacity)
	{
		var alpha = alphaData[i];
		
		if (DensityTex != null)
		{
			var u = (x + alphaX) * alphaScaleX;
			var v = (y + alphaY) * alphaScaleY;
			
			if (u >= 0 && v >= 0 && u < 1.0f && v < 1.0f)
			{
				var density = DensityTex.GetPixel((int)(u * DensityTex.width), (int)(v * DensityTex.height));
				
				opacity *= 1.0f - density.a;
			}
		}
		
		var a = (int)(opacity * 255.0f);
		
		alpha = (byte)Mathf.Clamp((int)alpha - a, 0, 255);
		
		alphaData[i] = alpha;
		
		return alpha;
	}
	
	private void FastPaintBoth(int i, int x, int y, float opacity)
	{
		var color = alphaTex.GetPixel(x, y);
		var alpha = FastPaint(i, x, y, opacity);
		
		color.a = D2D_AlphaTex.ConvertAlpha(alpha);
		
		alphaTex.SetPixel(x, y, color);
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
	
	private void DeserializeAlphaTex()
	{
		if (dirty == true || alphaTex == null)
		{
			dirty = false;
			
			if (alphaData != null)
			{
				alphaTex = new Texture2D(alphaWidth, alphaHeight, TextureFormat.Alpha8, false);
				alphaTex.hideFlags = HideFlags.DontSave;
				alphaTex.wrapMode  = TextureWrapMode.Clamp;
				
				for (var y = 0; y < alphaHeight; y++)
				{
					for (var x = 0; x < alphaWidth; x++)
					{
						var color = default(Color);
						var alpha = alphaData[x + y * alphaWidth];
						
						color.a = D2D_AlphaTex.ConvertAlpha(alpha);
						
						alphaTex.SetPixel(x, y, color);
					}
				}
				
				alphaTex.Apply();
			}
			else
			{
				DestroyAlphaTex();
			}
		}
		
		if (alphaTex != null)
		{
			if (Binary == true)
			{
				if (alphaTex.filterMode != FilterMode.Point)
				{
					alphaTex.filterMode = FilterMode.Point;
				}
			}
			else
			{
				if (alphaTex.filterMode != FilterMode.Bilinear)
				{
					alphaTex.filterMode = FilterMode.Bilinear;
				}
			}
		}
	}
	
	private void DestroyAlphaTex()
	{
		if (alphaTex != null)
		{
			D2D_Helper.Destroy(alphaTex);
			
			alphaTex = null;
		}
		
		alphaData   = null;
		alphaWidth  = 0;
		alphaHeight = 0;
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
	
	private void NotifyChanges()
	{
		D2D_Helper.BroadcastMessage(transform, "OnAlphaTexReplaced", SendMessageOptions.DontRequireReceiver);
	}
	
	private void NotifyChanges(int xMin, int xMax, int yMin, int yMax)
	{
		var rect = new D2D_Rect();
		
		rect.XMin = xMin;
		rect.XMax = xMax;
		rect.YMin = yMin;
		rect.YMax = yMax;
		
		D2D_Helper.BroadcastMessage(transform, "OnAlphaTexModified", rect, SendMessageOptions.DontRequireReceiver);
	}
}