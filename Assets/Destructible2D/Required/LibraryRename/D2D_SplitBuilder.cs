using UnityEngine;
using System.Collections.Generic;

public enum D2D_SplitOrder
{
	Default,
	KeepLargest,
	KeepSmallest
}

public class D2D_SplitGroup
{
	public List<int> Indices = new List<int>(); // pixel index = x + y * width
	
	public List<byte> Alphas = new List<byte>();
	
	public int Count;
	
	public int XMin;
	
	public int XMax;
	
	public int YMin;
	
	public int YMax;
	
	public void AddToPool()
	{
		Indices.Clear();
		
		Alphas.Clear();
		
		Count = 0;
	}
	
	public void AddTriangle(D2D_Point a, D2D_Point b, D2D_Point c)
	{
		if (a.Y != b.Y || a.Y != c.Y)
		{
			// Make a highest, and c lowest
			if (b.Y > a.Y) D2D_Helper.Swap(ref a, ref b);
			if (c.Y > a.Y) D2D_Helper.Swap(ref c, ref a);
			if (c.Y > b.Y) D2D_Helper.Swap(ref b, ref c);
			
			var fth = a.Y - c.Y; // Full triangle height
			var tth = a.Y - b.Y; // Top triangle height
			var bth = b.Y - c.Y; // Bottom triangle height
			
			// Find a to c intercept along b plane
			var inx = c.X + (a.X - c.X) * D2D_Helper.Divide(bth, fth);
			var d   = new D2D_Point((int)inx, b.Y);
			
			// Top triangle
			var abs = D2D_Helper.Divide(a.X - b.X, tth); // A/B slope
			var ads = D2D_Helper.Divide(a.X - d.X, tth); // A/D slope
			
			AddTriangle(b.X, d.X, abs, ads, b.Y, 1, tth);
			
			// Bottom triangle
			var cbs = D2D_Helper.Divide(c.X - b.X, bth); // C/B slope
			var cds = D2D_Helper.Divide(c.X - d.X, bth); // C/D slope
			
			AddTriangle(b.X, d.X, cbs, cds, b.Y, -1, bth);
		}
	}
	
	public void AddTriangle(float l, float r, float ls, float rs, int y, int s, int c) // left x, right x, left slope, right slope, y, sign, count
	{
		if (l > r)
		{
			D2D_Helper.Swap(ref l, ref r);
			D2D_Helper.Swap(ref ls, ref rs);
		}
		
		for (var i = 0; i < c; i++)
		{
			var il = Mathf.FloorToInt(l);
			var ir = Mathf.CeilToInt(r);
			
			for (var x = il; x < ir; x++)
			{
				AddPixel(x, y);
			}
			
			y += s;
			l += ls;
			r += rs;
		}
	}
	
	public void AddPixel(int x, int y)
	{
		var color = D2D_SplitBuilder.AlphaTex.GetPixel(x, y);
		
		AddPixel(x, y, D2D_AlphaTex.ConvertAlpha(color.a));
	}
	
	public void AddPixel(int x, int y, float mul)
	{
		var color = D2D_SplitBuilder.AlphaTex.GetPixel(x, y);
		
		AddPixel(x, y, D2D_AlphaTex.ConvertAlpha(color.a * mul));
	}
	
	public void AddPixel(int x, int y, byte alpha)
	{
		if (x >= 0 && x < D2D_SplitBuilder.AlphaTexWidth && y >= 0 && y < D2D_SplitBuilder.AlphaTexHeight)
		{
			if (Count > 0)
			{
				     if (x < XMin) XMin = x;
				else if (x > XMax) XMax = x;
				
				     if (y < YMin) YMin = y;
				else if (y > YMax) YMax = y;
			}
			else
			{
				XMin = XMax = x;
				YMin = YMax = y;
			}
			
			Indices.Add(x + y * D2D_SplitBuilder.AlphaTexWidth);
			
			Alphas.Add(alpha);
			
			Count += 1;
		}
	}
}

public static class D2D_SplitBuilder
{
	public static List<D2D_SplitGroup> Groups = new List<D2D_SplitGroup>();
	
	public static Texture2D AlphaTex;
	
	public static int AlphaTexWidth;
	
	public static int AlphaTexHeight;
	
	private static D2D_Destructible destructible;
	
	private static D2D_SplitData splitData = new D2D_SplitData();
	
	public static D2D_SplitGroup CreateGroup()
	{
		if (destructible != null)
		{
			var group = D2D_ClassPool<D2D_SplitGroup>.Pop() ?? new D2D_SplitGroup();
			
			Groups.Add(group);
			
			return group;
		}
		
		return null;
	}
	
	public static void DiscardTinyBits(int minPixels)
	{
		for (var i = Groups.Count - 1; i >= 0; i--)
		{
			var group = Groups[i];
			
			if (group.Count < minPixels)
			{
				D2D_ClassPool<D2D_SplitGroup>.Add(group, g => g.AddToPool());
				
				Groups.RemoveAt(i);
			}
		}
	}
	
	public static void BeginSplitting(D2D_Destructible newDestructible)
	{
		if (newDestructible != null)
		{
			D2D_ClassPool<D2D_SplitGroup>.Add(Groups, g => g.AddToPool()); // EndSplitting may not get called, so call here in case
			
			destructible = newDestructible;
			
			AlphaTex = newDestructible.AlphaTex;
			
			AlphaTexWidth  = AlphaTex.width;
			AlphaTexHeight = AlphaTex.height;
		}
	}
	
	public static void EndSplitting(D2D_SplitOrder order)
	{
		if (destructible != null)
		{
			if (Groups.Count > 0)
			{
				// Sort
				switch (order)
				{
					case D2D_SplitOrder.KeepLargest:  Groups.Sort((a, b) => b.Count.CompareTo(a.Count)); break;
					case D2D_SplitOrder.KeepSmallest: Groups.Sort((a, b) => a.Count.CompareTo(b.Count)); break;
				}
				
				// Store list of others
				splitData.SolidPixelCounts.Clear();
				
				for (var i = 0; i < Groups.Count; i++)
				{
					splitData.SolidPixelCounts.Add(Groups[i].Count);
				}
				
				// Split
				for (var i = Groups.Count - 1; i >= 0; i--)
				{
					var group = Groups[i];
					
					splitData.Index   = i;
					splitData.IsClone = i > 0;
					
					// Split
					if (i > 0)
					{
						var tempAlphaData = destructible.AlphaData;
						
						destructible.AlphaData = null;
						
						// Clone this destructible without alpha data, because we will manually set it after this
						var clonedDestructible = D2D_Helper.CloneGameObject(destructible.gameObject, destructible.transform.parent).GetComponent<D2D_Destructible>();
						
						destructible.AlphaData = tempAlphaData;
						
						Split(clonedDestructible, group, true);
					}
					// Overwrite original
					else
					{
						Split(destructible, group, false);
					}
				}
			}
			else
			{
				D2D_Helper.Destroy(destructible.gameObject);
			}
			
			destructible = null;
		}
		
		D2D_ClassPool<D2D_SplitGroup>.Add(Groups, g => g.AddToPool());
	}
	
	private static void Split(D2D_Destructible destructible, D2D_SplitGroup group, bool isClone)
	{
		var subX      = group.XMin;
		var subY      = group.YMin;
		var subWidth  = group.XMax - group.XMin + 1;
		var subHeight = group.YMax - group.YMin + 1;
		var subTotal  = subWidth * subHeight;
		var subAlpha  = new byte[subTotal];
		
		for (var i = group.Count - 1; i >= 0; i--)
		{
			var j = group.Indices[i];
			var a = group.Alphas[i];
			var x = (j % AlphaTexWidth) - subX;
			var y = (j / AlphaTexWidth) - subY;
			var s = x + y * subWidth;
			
			subAlpha[s] = a;
		}
		
		destructible.SubsetAlphaWith(subAlpha, subWidth, subHeight, subX, subY);
		
		// Split notification
		D2D_Helper.BroadcastMessage(destructible.transform, "OnDestructibleSplit", splitData, SendMessageOptions.DontRequireReceiver);
	}
}