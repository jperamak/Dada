using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Quad Fracturer")]
public class D2D_QuadFracturer : D2D_Fracturer
{
	[D2D_RangeAttribute(0.0f, 0.5f)]
	public float Irregularity = 0.25f;
	
	private static List<D2D_Quad> quads = new List<D2D_Quad>();
	
	protected override void DoFracture()
	{
		var mainQuad = new D2D_Quad();
		
		mainQuad.BL = new D2D_Point(0, 0);
		mainQuad.BR = new D2D_Point(width - 1, 0);
		mainQuad.TL = new D2D_Point(0, height - 1);
		mainQuad.TR = new D2D_Point(width - 1, height - 1);
		
		quads.Clear();
		quads.Add(mainQuad);
		
		for (var i = 0; i < Count; i++)
		{
			SplitLargest();
		}
		
		for (var i = 0; i < quads.Count; i++)
		{
			var quad  = quads[i];
			var group = D2D_SplitBuilder.CreateGroup();
			
			group.AddTriangle(quad.BL, quad.BR, quad.TL);
			group.AddTriangle(quad.TR, quad.TL, quad.BR);
		}
	}
	
	private void SplitLargest()
	{
		var largestIndex = 0;
		var largestSize  = 0;
		
		for (var i = 0; i < quads.Count; i++)
		{
			var quad = quads[i];
			
			if (quad.Size > largestSize)
			{
				largestIndex = i;
				largestSize  = quad.Size;
			}
		}
		
		var largestQuad = quads[largestIndex];
		var left        = default(D2D_Quad);
		var right       = default(D2D_Quad);
		
		largestQuad.Split(ref left, ref right, Irregularity);
		
		quads.Remove(largestQuad);
		quads.Add(left);
		quads.Add(right);
	}
}