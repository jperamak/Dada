using UnityEngine;
using System.Collections.Generic;

public static class D2D_AlphaTex
{
	private static float ReciprocalOf255 = 1.0f / 255.0f;
	
	private static byte[] data = new byte[0];
	
	private static byte[] tempData = new byte[0];
	
	private static int width;
	
	private static int height;
	
	private static int total;
	
	public static byte[] Data
	{
		get
		{
			return data;
		}
	}
	
	public static int Width
	{
		get
		{
			return width;
		}
	}
	
	public static int Height
	{
		get
		{
			return height;
		}
	}
	
	public static float ConvertAlpha(byte a)
	{
		return (float)(ReciprocalOf255 * (float)a);
	}
	
	public static byte ConvertAlpha(float a)
	{
		return (byte)(255.0f * a);
	}
	
	public static void Load(byte[] newData, int newWidth, int newHeight)
	{
		var newTotal = newWidth * newHeight;
		
		if (newData != null && newWidth >= 0 && newHeight >= 0 && newData.Length >= newTotal)
		{
			if (data.Length < newTotal)
			{
				data = new byte[newTotal];
			}
			
			width  = newWidth;
			height = newHeight;
			total  = newTotal;
			
			for (var i = 0; i < total; i++)
			{
				data[i] = newData[i];
			}
		}
	}
	
	public static byte Get(int x, int y)
	{
		return data[x + y * width];
	}
	
	public static void Set(int x, int y, byte a)
	{
		data[x + y * width] = a;
	}
	
	public static byte GetClamp(int x, int y)
	{
		if (x < 0) x = 0; else if (x >= width ) x = width  -1;
		if (y < 0) y = 0; else if (y >= height) y = height -1;
		
		return Get(x, y);
	}
	
	public static byte GetDefault(int x, int y)
	{
		if (x >= 0 && x < width && y >= 0 && y < height)
		{
			return Get(x, y);
		}
		
		return 0;
	}
	
	public static byte GetBilinear(float u, float v)
	{
		u = u * (width  - 1);
		v = v * (height - 1);
		
		var x = Mathf.FloorToInt(u);
		var y = Mathf.FloorToInt(v);
		var s = u - x;
		var t = v - y;
		
		var bl = GetClamp(x    , y    );
		var br = GetClamp(x + 1, y    );
		var tl = GetClamp(x    , y + 1);
		var tr = GetClamp(x + 1, y + 1);
		
		var bb = Lerp(bl, br, s);
		var tt = Lerp(tl, tr, s);
		
		return Lerp(bb, tt, t);
	}
	
	public static byte Lerp(byte a, byte b, float t)
	{
		var i = 1.0f - t;
		
		return (byte)((a * i) + (b * t));
	}
	
	public static void Resize(int newWidth, int newHeight)
	{
		if (newWidth >= 0 && newHeight >= 0 && width > 0 && height > 0)
		{
			var newTotal = newWidth * newHeight;
			
			if (tempData.Length < newTotal)
			{
				tempData = new byte[newTotal];
			}
			
			var w = (float)(newWidth - 1);
			var h = (float)(newHeight - 1);
			
			for (var y = 0; y < newHeight; y++)
			{
				for (var x = 0; x < newWidth; x++)
				{
					tempData[x + y * newWidth] = GetBilinear((float)x / w, (float)y / h);
				}
			}
			
			width  = newWidth;
			height = newHeight;
			total  = newTotal;
			
			D2D_Helper.Swap(ref data, ref tempData);
		}
	}
	
	public static void Halve()
	{
		if (width > 0 && height > 0)
		{
			var newWidth  = width  / 2;
			var newHeight = height / 2;
			
			if (newWidth >= 0 && newHeight >= 0)
			{
				var newTotal = newWidth * newHeight;
				
				if (tempData.Length < newTotal)
				{
					tempData = new byte[newTotal];
				}
				
				for (var y = 0; y < newHeight; y++)
				{
					var y0 = y * 2;
					var y1 = y0 + 1;
					
					for (var x = 0; x < newWidth; x++)
					{
						var x0 = x * 2;
						var x1 = x0 + 1;
						
						var a = GetClamp(x0, y0);
						var b = GetClamp(x1, y0);
						var c = GetClamp(x0, y1);
						var d = GetClamp(x1, y1);
						var t = (int)a + (int)b + (int)c + (int)d;
						
						tempData[x + y * newWidth] = (byte)(t / 4);
					}
				}
				
				width  = newWidth;
				height = newHeight;
				total  = newTotal;
				
				D2D_Helper.Swap(ref data, ref tempData);
			}
		}
	}
	
	public static void Blur()
	{
		BlurHorizontally();
		BlurVertically();
	}
	
	public static void BlurHorizontally()
	{
		if (width > 0 && height > 0)
		{
			if (tempData.Length < total)
			{
				tempData = new byte[total];
			}
			
			for (var y = 0; y < height; y++)
			{
				for (var x = 0; x < width; x++)
				{
					var a = GetDefault(x - 1, y);
					var b = GetDefault(x    , y);
					var c = GetDefault(x + 1, y);
					var t = (int)a + (int)b + (int)c;
					
					tempData[x + y * width] = (byte)(t / 3);
				}
			}
			
			D2D_Helper.Swap(ref data, ref tempData);
		}
	}
	
	public static void BlurVertically()
	{
		if (width > 0 && height > 0)
		{
			if (tempData.Length < total)
			{
				tempData = new byte[total];
			}
			
			for (var y = 0; y < height; y++)
			{
				for (var x = 0; x < width; x++)
				{
					var a = GetDefault(x, y - 1);
					var b = GetDefault(x, y    );
					var c = GetDefault(x, y + 1);
					var t = (int)a + (int)b + (int)c;
					
					tempData[x + y * width] = (byte)(t / 3);
				}
			}
			
			D2D_Helper.Swap(ref data, ref tempData);
		}
	}
}