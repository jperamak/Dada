using UnityEngine;

[System.Serializable]
public class D2D_Pixels
{
	[SerializeField]
	private int width;
	
	[SerializeField]
	private int height;
	
	[SerializeField]
	private Color32[] pixels;
	
	public int Width
	{
		get
		{
			return width;
		}
	}
	
	public int Height
	{
		get
		{
			return height;
		}
	}
	
	public Color32[] Pixels
	{
		get
		{
			return pixels;
		}
	}
	
	public D2D_Pixels()
	{
	}
	
	public D2D_Pixels(Texture texture) : this((Texture2D)texture)
	{
	}
	
	public D2D_Pixels(Sprite sprite)
	{
		if (sprite == null) throw new System.ArgumentNullException();
		
#if UNITY_EDITOR
		D2D_Helper.MakeTextureReadable(sprite.texture);
#endif
		
		var rect         = sprite.textureRect;
		var sourceWidth  = sprite.texture.width;
		var sourcePixels = sprite.texture.GetPixels32();
		var sourceOffset = sourceWidth * (int)rect.yMin + (int)rect.xMin;
		var targetOffset = 0;
		
		width  = (int)rect.width;
		height = (int)rect.height;
		pixels = new Color32[width * height];
		
		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				pixels[targetOffset + x] = sourcePixels[sourceOffset + x];
			}
			
			sourceOffset += sourceWidth;
			targetOffset += width;
		}
	}
	
	public D2D_Pixels(Texture2D texture)
	{
		if (texture == null) throw new System.ArgumentNullException();
		
#if UNITY_EDITOR
		D2D_Helper.MakeTextureReadable(texture);
#endif
		
		width  = texture.width;
		height = texture.height;
		pixels = texture.GetPixels32();
	}
	
	public D2D_Pixels(int newWidth, int newHeight)
	{
		if (newWidth  < 0) throw new System.ArgumentOutOfRangeException();
		if (newHeight < 0) throw new System.ArgumentOutOfRangeException();
		
		width  = newWidth;
		height = newHeight;
		pixels = new Color32[newWidth * newHeight];
	}
	
	public D2D_Pixels(int newWidth, int newHeight, Color32[] newPixels)
	{
		if (newWidth < 0) throw new System.ArgumentOutOfRangeException();
		
		if (newHeight < 0) throw new System.ArgumentOutOfRangeException();
		
		if (newPixels == null) throw new System.ArgumentNullException();
		
		if (newWidth * newHeight != newPixels.Length) throw new System.ArgumentOutOfRangeException();
		
		width  = newWidth;
		height = newHeight;
		pixels = newPixels;
	}
	
	public Color32 GetPixel(int x, int y)
	{
		return pixels[x + width * y];
	}
	
	public Color32 GetPixelTransparent(int x, int y)
	{
		if (x < 0 || y < 0 || x >= width || y >= height) return new Color32(0, 0, 0, 0);
		
		return pixels[x + width * y];
	}
	
	public Color32 GetPixelClamp(int x, int y)
	{
		if (x < 0) x = 0; else if (x >= width ) x = width  -1;
		if (y < 0) y = 0; else if (y >= height) y = height -1;
		
		return pixels[x + width * y];
	}
	
	public Color32 GetPixelRepeat(int x, int y)
	{
		x = x >= 0 ? x % width  : width  + (x % width );
		y = y >= 0 ? y % height : height + (y % height);
		
		return pixels[x + width * y];
	}
	
	public Color32 GetPixelBilinear(float u, float v)
	{
		u = u * (width - 1);
		v = v * (height - 1);
		
		var x = Mathf.FloorToInt(u);
		var y = Mathf.FloorToInt(v);
		var s = u - x;
		var t = v - y;
		
		var bl = GetPixelClamp(x    , y    );
		var br = GetPixelClamp(x + 1, y    );
		var tl = GetPixelClamp(x    , y + 1);
		var tr = GetPixelClamp(x + 1, y + 1);
		
		var bb = Color32.Lerp(bl, br, s);
		var tt = Color32.Lerp(tl, tr, s);
		
		return Color32.Lerp(bb, tt, t);
	}
	
	public void SetPixel(int x, int y, Color32 colour)
	{
		pixels[x + width * y] = colour;
	}
	
	public void SetPixelClamp(int x, int y, Color32 colour)
	{
		if (x < 0) x = 0; else if (x >= width ) x = width  -1;
		if (y < 0) y = 0; else if (y >= height) y = height -1;
		
		pixels[x + width * y] = colour;
	}
	
	public void SetPixels(int x, int y, D2D_Pixels s)
	{
		for (var sy = 0; sy < s.height; sy++)
		{
			for (var sx = 0; sx < s.width; sx++)
			{
				SetPixel(x + sx, y + sy, s.GetPixel(sx, sy));
			}
		}
	}
	
	public void SetPixelsClamp(int x, int y, D2D_Pixels s)
	{
		for (var sy = 0; sy < s.height; sy++)
		{
			for (var sx = 0; sx < s.width; sx++)
			{
				SetPixelClamp(x + sx, y + sy, s.GetPixelClamp(sx, sy));
			}
		}
	}
	
	public D2D_Pixels GetResized(int newWidth, int newHeight)
	{
		var o = new D2D_Pixels(newWidth, newHeight);
		var w = (float)(newWidth - 1);
		var h = (float)(newHeight - 1);
		
		for (var y = 0; y < newHeight; y++)
		{
			for (var x = 0; x < newWidth; x++)
			{
				var pixel = GetPixelBilinear((float)x / w, (float)y / h);
				
				o.SetPixel(x, y, pixel);
			}
		}
		
		return o;
	}
	
	public D2D_Pixels GetBlurredAlpha()
	{
		var o = new D2D_Pixels(width, height);
		
		// Horizontal
		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				var a = GetPixelTransparent(x - 1, y);
				var b = GetPixelTransparent(x    , y);
				var c = GetPixelTransparent(x + 1, y);
				var t = (int)a.a + (int)b.a + (int)c.a;
				
				b.a = (byte)(t / 3);
				
				o.SetPixel(x, y, b);
			}
		}
		
		// Vertical
		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				var a = GetPixelTransparent(x, y - 1);
				var b = GetPixelTransparent(x, y    );
				var c = GetPixelTransparent(x, y + 1);
				var t = (int)a.a + (int)b.a + (int)c.a;
				
				b.a = (byte)(t / 3);
				
				o.SetPixel(x, y, b);
			}
		}
		
		return o;
	}
	
	public Texture2D Apply(TextureFormat format, bool mipmap = false, bool linear = false)
	{
		var texture = new Texture2D(width, height, format, mipmap, linear);
		
		texture.SetPixels32(pixels);
		texture.Apply();
		
		return texture;
	}
	
	public byte[] ApplyAlpha()
	{
		var alphas = new byte[pixels.Length];
		
		for (var i = 0; i < pixels.Length; i++)
		{
			alphas[i] = pixels[i].a;
		}
		
		return alphas;
	}
}