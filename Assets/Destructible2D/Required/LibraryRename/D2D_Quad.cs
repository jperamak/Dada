using UnityEngine;

[System.Serializable]
public class D2D_Quad
{
	public D2D_Point BL;
	public D2D_Point BR;
	public D2D_Point TL;
	public D2D_Point TR;
	public int       Size;
	
	public void CalculateSize()
	{
		var minX = Mathf.Min(Mathf.Min(BL.X, BR.X), Mathf.Min(TL.X, TR.X));
		var minY = Mathf.Min(Mathf.Min(BL.Y, BR.Y), Mathf.Min(TL.Y, TR.Y));
		var maxX = Mathf.Max(Mathf.Max(BL.X, BR.X), Mathf.Max(TL.X, TR.X));
		var maxY = Mathf.Max(Mathf.Max(BL.Y, BR.Y), Mathf.Max(TL.Y, TR.Y));
		
		Size = (maxX - minX) * (maxY - minY);
	}
	
	public void Split(ref D2D_Quad first, ref D2D_Quad second, float irregularity)
	{
		if (first  == null) first  = new D2D_Quad();
		if (second == null) second = new D2D_Quad();
		
		var b = D2D_Point.DistanceSq(BL, BR);
		var t = D2D_Point.DistanceSq(TL, TR);
		var l = D2D_Point.DistanceSq(BL, TL);
		var r = D2D_Point.DistanceSq(BR, TR);
		
		// Vertical split
		if (b > l || b > t || t > l || t > r)
		{
			var TS = TL + (TR - TL) * Random.Range(0.5f - irregularity, 0.5f + irregularity);
			var BS = BL + (BR - BL) * Random.Range(0.5f - irregularity, 0.5f + irregularity);
			
			first.BL = BL;
			first.BR = BS;
			first.TL = TL;
			first.TR = TS;
			first.CalculateSize();
			
			second.BL = BS;
			second.BR = BR;
			second.TL = TS;
			second.TR = TR;
			second.CalculateSize();
		}
		// Horizontal split
		else
		{
			var LS = BL + (TL - BL) * Random.Range(0.5f - irregularity, 0.5f + irregularity);
			var RS = BR + (TR - BR) * Random.Range(0.5f - irregularity, 0.5f + irregularity);
			
			first.BL = LS;
			first.BR = RS;
			first.TL = TL;
			first.TR = TR;
			first.CalculateSize();
			
			second.BL = BL;
			second.BR = BR;
			second.TL = LS;
			second.TR = RS;
			second.CalculateSize();
		}
	}
}