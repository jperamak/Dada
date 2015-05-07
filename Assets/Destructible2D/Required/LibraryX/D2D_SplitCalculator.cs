using UnityEngine;
using System.Collections.Generic;

public static class D2D_SplitCalculator
{
	class Fill
	{
		public List<int> Indices = new List<int>();
		
		public List<Color32> Colours = new List<Color32>();
		
		public int Count;
		
		public int XMin;
		
		public int XMax;
		
		public int YMin;
		
		public int YMax;
		
		public bool Valid;
	}
	
	class Spread
	{
		public int i;
		public int x;
		public int y;
	}
	
	private static D2D_Destructible target;
	
	private static List<bool> cells = new List<bool>();
	
	private static List<Fill> fills = new List<Fill>();
	
	private static List<Spread> spreads = new List<Spread>();
	
	private static int spreadCount;
	
	private static Fill currentFill;
	
	private static Color32[] pixels;
	
	private static Texture2D tex;
	
	private static int width;
	
	private static int height;
	
	private static int total;
	
	public static bool Generate(D2D_Destructible destructible, D2D_SpriteSplitOrder splitOrder)
	{
		cells.Clear();
		
		if (destructible != null && destructible.AlphaTex != null)
		{
			target = destructible;
			tex    = target.AlphaTex;
			width  = tex.width;
			height = tex.height;
			total  = width * height;
			pixels = tex.GetPixels32();
			
			if (cells.Capacity < total)
			{
				cells.Capacity = total;
			}
			
			var threshold = (byte)(target.SplitThreshold * 255.0f);
			
			for (var i = 0; i < total; i++)
			{
				cells.Add(pixels[i].a >= threshold);
			}
			
			fills.Clear();
			
			var validFillCount = 0;
			
			for (var i = 0; i < total; i++)
			{
				if (cells[i] == true)
				{
					currentFill = new Fill(); fills.Add(currentFill);
					
					currentFill.XMin = currentFill.XMax = i % width;
					currentFill.YMin = currentFill.YMax = i / width;
					
					BeginFloodFill(i, currentFill.XMin, currentFill.YMin);
					
					// Skip the first floodfill
					if (currentFill.Count >= target.SplitMinPixels)
					{
						currentFill.Valid = true; validFillCount += 1;
					}
				}
			}
			
			// Can we split?
			if (validFillCount > 1)
			{
				var firstSet = false;
				
				switch (splitOrder)
				{
					case D2D_SpriteSplitOrder.KeepLargest:  fills.Sort((a, b) => b.Count.CompareTo(a.Count)); break;
					case D2D_SpriteSplitOrder.KeepSmallest: fills.Sort((a, b) => a.Count.CompareTo(b.Count)); break;
				}
				
				foreach (var fill in fills)
				{
					if (fill.Valid == true)
					{
						if (firstSet == false)
						{
							firstSet = true;
							
							Split(destructible, fill, false);
						}
						else
						{
							var clonedGameObject   = D2D_Helper.CloneGameObject(destructible.gameObject, destructible.transform.parent);
							var clonedDestructible = clonedGameObject.GetComponent<D2D_Destructible>();
							
							Split(clonedDestructible, fill, true);
						}
					}
				}
				
				return true;
			}
			
			if (validFillCount == 0)
			{
				D2D_Helper.Destroy(destructible.gameObject);
			}
		}
		
		return false;
	}
	
	private static void Split(D2D_Destructible destructible, Fill fill, bool isClone)
	{
		var clear = new Color32(0, 0, 0, 0);
		
		for (var i = 0; i < total; i++)
		{
			pixels[i] = clear;
		}
		
		for (var i = 0; i < fill.Count; i++)
		{
			pixels[fill.Indices[i]] = fill.Colours[i];
		}
		
		destructible.ReplaceAlphaWith(width, height, pixels);
		
		// Split notification
		destructible.BroadcastMessage("OnSpriteSplit", isClone, SendMessageOptions.DontRequireReceiver);
	}
	
	private static void BeginFloodFill(int i, int x, int y)
	{
		var oldSpreadCount = spreadCount = 0;
		
		SpreadTo(i, x, y);
		
		// Non-recursive floodfill
		while (spreadCount != oldSpreadCount)
		{
			var start = oldSpreadCount;
			var end   = oldSpreadCount = spreadCount;
			
			for (var j = start; j < end; j++)
			{
				var spread = spreads[j];
				
				FloodFill(spread.i, spread.x, spread.y);
			}
		}
	}
	
	private static void SpreadTo(int i, int x, int y)
	{
		cells[i] = false;
		
		var spread = default(Spread);
		
		if (spreadCount >= spreads.Count)
		{
			spread = new Spread(); spreads.Add(spread);
		}
		else
		{
			spread = spreads[spreadCount];
		}
		
		spread.i = i;
		spread.x = x;
		spread.y = y;
		
		spreadCount += 1;
	}
	
	private static void FloodFill(int i, int x, int y)
	{
		currentFill.Count += 1;
		currentFill.Indices.Add(i);
		currentFill.Colours.Add(pixels[i]);
		
		currentFill.XMin = Mathf.Min(currentFill.XMin, x);
		currentFill.XMax = Mathf.Max(currentFill.XMax, x);
		currentFill.YMin = Mathf.Min(currentFill.YMin, y);
		currentFill.YMax = Mathf.Max(currentFill.YMax, y);
		
		// Left
		if (x > 0)
		{
			var n = i - 1;
			
			if (cells[n] == true)
			{
				SpreadTo(n, x - 1, y);
			}
		}
		
		// Right
		if (x < width - 1)
		{
			var n = i + 1;
			
			if (cells[n] == true)
			{
				SpreadTo(n, x + 1, y);
			}
		}
		
		// Bottom
		if (y > 0)
		{
			var n = i - width;
			
			if (cells[n] == true)
			{
				SpreadTo(n, x, y - 1);
			}
		}
		
		// Top
		if (y < height - 1)
		{
			var n = i + width;
			
			if (cells[n] == true)
			{
				SpreadTo(n, x, y + 1);
			}
		}
	}
}