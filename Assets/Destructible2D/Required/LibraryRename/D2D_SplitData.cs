using System.Collections.Generic;

public class D2D_SplitData
{
	public int Index;
	
	public bool IsClone;
	
	public List<int> SolidPixelCounts = new List<int>();
	
	public int SolidPixelThresholdTotal(int threshold)
	{
		var total = 0;
		
		for (var i = SolidPixelCounts.Count - 1; i >= 0; i--)
		{
			if (SolidPixelCounts[i] >= threshold)
			{
				total += 1;
			}
		}
		
		return total;
	}
}