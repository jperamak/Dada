using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(D2D_Destructible))]
[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Splittable")]
public class D2D_Splittable : MonoBehaviour
{
	class Spread
	{
		public int i;
		public int x;
		public int y;
	}
	
	[D2D_RangeAttribute(0.0f, 1.0f)]
	public float Threshold = 0.5f;
	
	public int MinPixels = 20;
	
	public D2D_SplitOrder SplitOrder = D2D_SplitOrder.Default;
	
	public static bool BusySplitting;
	
	private D2D_Destructible destructible;
	
	private static List<bool> cells = new List<bool>();
	
	private static List<Spread> spreads = new List<Spread>();
	
	private static int spreadCount;
	
	private static Texture2D alphaTex;
	
	private static int width;
	
	private static int height;
	
	private static int total;
	
	private static D2D_SplitGroup splitGroup;
	
	[ContextMenu("Update Split")]
	public void UpdateSplit()
	{
		if (destructible == null) destructible = GetComponent<D2D_Destructible>();
		
		BeginSplitting();
		{
			// Find which pixels are solid
			for (var y = 0; y < height; y++)
			{
				for (var x = 0; x < width; x++)
				{
					cells.Add(alphaTex.GetPixel(x, y).a >= Threshold);
				}
			}
			
			// Go through all pixels
			for (var i = 0; i < total; i++)
			{
				// First pixel of unclaimed island?
				if (cells[i] == true)
				{
					splitGroup = D2D_SplitBuilder.CreateGroup();
					
					BeginFloodFill(i, i % width, i / width);
				}
			}
		}
		EndSplitting();
	}
	
	// Show enable/disable toggle in inspector
	protected virtual void Start()
	{
	}
	
	protected virtual void OnAlphaTexModified()
	{
		if (BusySplitting == false) // Prevent recursive calling
		{
			if (D2D_Helper.Enabled(this) == true)
			{
				UpdateSplit();
			}
		}
	}
	
	private void BeginSplitting()
	{
		BusySplitting = true;
		
		alphaTex = destructible.AlphaTex;
		width    = alphaTex.width;
		height   = alphaTex.height;
		total    = width * height;
		
		// Clear cells and set capacity
		cells.Clear();
		
		if (cells.Capacity < total)
		{
			cells.Capacity = total;
		}
		
		D2D_SplitBuilder.BeginSplitting(destructible);
	}
	
	private void EndSplitting()
	{
		// If there's only one island then skip, else every modification will waste CPU
		if (D2D_SplitBuilder.Groups.Count != 1)
		{
			D2D_SplitBuilder.DiscardTinyBits(MinPixels);
			
			D2D_SplitBuilder.EndSplitting(SplitOrder);
		}
		
		BusySplitting = false;
	}
	
	private static void BeginFloodFill(int i, int x, int y)
	{
		var oldSpreadsCount = spreadCount = 0;
		
		SpreadTo(i, x, y);
		
		// Non-recursive floodfill
		while (spreadCount != oldSpreadsCount)
		{
			var start = oldSpreadsCount;
			var end   = oldSpreadsCount = spreadCount;
			
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
		
		spreadCount += 1;
		
		spread.i = i;
		spread.x = x;
		spread.y = y;
		
		splitGroup.AddPixel(x, y);
	}
	
	private static void FloodFill(int i, int x, int y)
	{
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