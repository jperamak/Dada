using UnityEngine;
using System.Collections.Generic;

public static class D2D_ColliderBuilder
{
	enum Edge
	{
		Left,
		Right,
		Bottom,
		Top
	}
	
	class Point
	{
		public bool Used;
		
		public Point Other;
		
		public Vector2 Position;
		
		public Edge Inner;
		
		public Edge Outer;
	}
	
	class Cell
	{
		public int X;
		
		public int Y;
		
		public int PointIndex;
		
		public int PointCount;
	}
	
	private static int xMin;
	
	private static int xMax;
	
	private static int yMin;
	
	private static int yMax;
	
	private static int width;
	
	private static int height;
	
	private static List<Cell> cells = new List<Cell>();
	
	private static int cellCount;
	
	private static int cellPointCount;
	
	private static List<Point> cellPoints = new List<Point>();
	
	private static D2D_LinkedList<Point> tracedEdges = new D2D_LinkedList<Point>();
	
	private static Vector2 translation;
	
	private static byte[] pixels;
	
	private static int pixelsL;
	
	private static int pixelsR;
	
	private static int pixelsB;
	
	private static int pixelsT;
	
	private static int pixelsW;
	
	public static void Calculate(D2D_Destructible destructible, int newXMin, int newXMax, int newYMin, int newYMax, bool cutEdges, bool binary)
	{
		cellCount      = 0;
		cellPointCount = 0;
		
		if (destructible != null)
		{
			if (cutEdges == true)
			{
				xMin = newXMin; if (xMin == 0) xMin -= 1; // Expand left?
				yMin = newYMin; if (yMin == 0) yMin -= 1; // Expand bottom?
				xMax = newXMax;
				yMax = newYMax;
				
				GetPixels(destructible, newXMin - 1, newYMin - 1, newXMax + 1, newYMax + 1);
			}
			else
			{
				xMin = newXMin - 1;
				yMin = newYMin - 1;
				xMax = newXMax + 1;
				yMax = newYMax + 1;
				
				GetPixels(destructible, newXMin, newYMin, newXMax, newYMax);
			}
			
			width  = xMax - xMin;
			height = yMax - yMin;
			
			if (width > 0 && height > 0)
			{
				if (cells.Capacity < width * height)
				{
					cells.Capacity = width * height;
				}
				
				for (var y = 0; y < height; y++)
				{
					var y0 = yMin + y;
					var y1 = y0 + 1;
					
					for (var x = 0; x < width; x++)
					{
						var x0 = xMin + x;
						var x1 = x0 + 1;
						
						var cell = GetNextCell();
						var bl   = GetAlphaOrDefault(x0, y0);
						var br   = GetAlphaOrDefault(x1, y0);
						var tl   = GetAlphaOrDefault(x0, y1);
						var tr   = GetAlphaOrDefault(x1, y1);
						
						if (binary == true)
						{
							bl = bl > 0.5f ? 1.0f : 0.0f;
							br = br > 0.5f ? 1.0f : 0.0f;
							tl = tl > 0.5f ? 1.0f : 0.0f;
							tr = tr > 0.5f ? 1.0f : 0.0f;
						}
						
						cell.X = x;
						cell.Y = y;
						
						translation.x = x0 + 0.5f;
						translation.y = y0 + 0.5f;
						
						CalculateCell(cell, bl, br, tl, tr, 0.5f);
					}
				}
			}
		}
	}
	
	public static void Build(GameObject gameObject, D2D_PolygonSpriteCollider.Cell colliderCell, float detail)
	{
		if (gameObject != null && colliderCell != null)
		{
			if (colliderCell.Collider == null)
			{
				colliderCell.Collider = gameObject.AddComponent<PolygonCollider2D>();
			}
			
			var pathCount = 0;
			
			for (var i = 0; i < cellCount; i++)
			{
				var cell = cells[i];
				
				for (var j = 0; j < cell.PointCount; j++)
				{
					var point = cellPoints[cell.PointIndex + j];
					
					if (point.Used == false)
					{
						TraceEdges(cell, point);
						
						WeldLines();
						OptimizeEdges(detail);
						
						if (tracedEdges.Count >= 3)
						{
							CompileTracedEdges(colliderCell, pathCount); pathCount += 1;
						}
					}
				}
			}
			
			colliderCell.Trim(pathCount);
		}
	}
	
	private static void CompileTracedEdges(D2D_PolygonSpriteCollider.Cell colliderCell, int pathCount)
	{
		if (colliderCell.Collider.pathCount <= pathCount)
		{
			colliderCell.Collider.pathCount += 1;
		}
		
		colliderCell.Collider.SetPath(pathCount, ExtractTracedEdges());
	}
	
	public static void Build(GameObject gameObject, D2D_EdgeSpriteCollider.Cell colliderCell, float detail)
	{
		if (gameObject != null && colliderCell != null)
		{
			var pathCount = 0;
			
			for (var i = 0; i < cellCount; i++)
			{
				var cell = cells[i];
				
				for (var j = 0; j < cell.PointCount; j++)
				{
					var point = cellPoints[cell.PointIndex + j];
					
					if (point.Used == false)
					{
						TraceEdges(cell, point);
						
						WeldLines();
						OptimizeEdges(detail);
						
						if (tracedEdges.Count >= 2)
						{
							CompileTracedEdges(gameObject, colliderCell, pathCount); pathCount += 1;
						}
					}
				}
			}
			
			colliderCell.Trim(pathCount);
		}
	}
	
	private static void CompileTracedEdges(GameObject gameObject, D2D_EdgeSpriteCollider.Cell colliderCell, int pathCount)
	{
		var collider = colliderCell.GetCollider(gameObject, pathCount);
		
		collider.points = ExtractTracedEdges();
	}
	
	public static Vector2[] ExtractTracedEdges()
	{
		var array = new Vector2[tracedEdges.Count];
		var node  = tracedEdges.First;
		var index = 0;
		
		while (node != null)
		{
			array[index++] = node.Value.Position;
			
			node = node.Next;
		}
		
		return array;
	}
	
	private static void GetPixels(D2D_Destructible destructible, int l, int b, int r, int t)
	{
		l = Mathf.Max(l, 0);
		b = Mathf.Max(b, 0);
		r = Mathf.Min(r, destructible.AlphaWidth);
		t = Mathf.Min(t, destructible.AlphaHeight);
		
		pixelsL = l;
		pixelsR = r;
		pixelsB = b;
		pixelsT = t;
		pixelsW = destructible.AlphaWidth;
		pixels  = destructible.AlphaData;
	}
	
	private static void CalculateCell(Cell cell, float bl, float br, float tl, float tr, float threshold)
	{
		var count = 0;
		var index = cellPointCount;
		var useBl = bl >= threshold;
		var useBr = br >= threshold;
		var useTl = tl >= threshold;
		var useTr = tr >= threshold;
		
		// Top
		if (useTl ^ useTr)
		{
			var point = GetNextCellPoint(); count += 1;
			
			point.Position = Transform((tl - threshold) / (tl - tr), 1.0f);
			point.Inner    = Edge.Top;
			point.Outer    = Edge.Bottom;
		}
		
		// Right
		if (useTr ^ useBr)
		{
			var point = GetNextCellPoint(); count += 1;
			
			point.Position = Transform(1.0f, (br - threshold) / (br - tr));
			point.Inner    = Edge.Right;
			point.Outer    = Edge.Left;
		}
		
		// Bottom
		if (useBl ^ useBr)
		{
			var point = GetNextCellPoint(); count += 1;
			
			point.Position = Transform((bl - threshold) / (bl - br), 0.0f);
			point.Inner    = Edge.Bottom;
			point.Outer    = Edge.Top;
		}
		
		// Left
		if (useTl ^ useBl)
		{
			var point = GetNextCellPoint(); count += 1;
			
			point.Position = Transform(0.0f, (bl - threshold) / (bl - tl));
			point.Inner    = Edge.Left;
			point.Outer    = Edge.Right;
		}
		
		// Pair up points
		switch (count)
		{
			case 2:
			{
				cellPoints[index + 0].Other = cellPoints[index + 1];
				cellPoints[index + 1].Other = cellPoints[index + 0];
			}
			break;
			
			case 4:
			{
				if (useTl == true && useBl == true)
				{
					cellPoints[index + 0].Other = cellPoints[index + 1];
					cellPoints[index + 1].Other = cellPoints[index + 0];
					cellPoints[index + 2].Other = cellPoints[index + 3];
					cellPoints[index + 3].Other = cellPoints[index + 2];
				}
				else
				{
					cellPoints[index + 0].Other = cellPoints[index + 3];
					cellPoints[index + 1].Other = cellPoints[index + 2];
					cellPoints[index + 2].Other = cellPoints[index + 1];
					cellPoints[index + 3].Other = cellPoints[index + 0];
				}
			}
			break;
		}
		
		cell.PointIndex = index;
		cell.PointCount = count;
	}
	
	private static Cell GetNextCell()
	{
		var cell = default(Cell);
		
		if (cellCount >= cells.Count)
		{
			cell = new Cell(); cells.Add(cell);
		}
		else
		{
			cell = cells[cellCount];
		}
		
		cellCount += 1;
		
		return cell;
	}
	
	private static Point GetNextCellPoint()
	{
		var point = default(Point);
		
		if (cellPointCount >= cellPoints.Count)
		{
			point = new Point(); cellPoints.Add(point);
		}
		else
		{
			point = cellPoints[cellPointCount];
			point.Used = false;
		}
		
		cellPointCount += 1;
		
		return point;
	}
	
	private static void TraceEdges(Cell cell, Point point)
	{
		tracedEdges.Clear();
		
		TraceEdge(cell, point, false);
		TraceEdge(cell, point.Other, true);
	}
	
	private static void TraceEdge(Cell cell, Point point, bool last)
	{
		point.Used = true;
		
		if (last == true)
		{
			tracedEdges.AddLast(point);
		}
		else
		{
			tracedEdges.AddFirst(point);
		}
		
		switch (point.Inner)
		{
			case Edge.Left:
			{
				cell = GetCell(cell.X - 1, cell.Y);
			}
			break;
			
			case Edge.Right:
			{
				cell = GetCell(cell.X + 1, cell.Y);
			}
			break;
			
			case Edge.Bottom:
			{
				cell = GetCell(cell.X, cell.Y - 1);
			}
			break;
			
			case Edge.Top:
			{
				cell = GetCell(cell.X, cell.Y + 1);
			}
			break;
		}
		
		if (cell != null)
		{
			for (var i = 0; i < cell.PointCount; i++)
			{
				var outerPoint = cellPoints[cell.PointIndex + i];
				
				if (outerPoint.Used == false && outerPoint.Inner == point.Outer)
				{
					outerPoint.Used = true;
					
					TraceEdge(cell, outerPoint.Other, last);
				}
			}
		}
	}
	
	private static void WeldLines()
	{
		if (tracedEdges.Count > 2)
		{
			var a = tracedEdges.First;
			var b = a.Next;
			var c = b.Next;
			var v = b.Value.Position - a.Value.Position;
			
			while (c != tracedEdges.Last)
			{
				var n = c.Value.Position - b.Value.Position;
				var z = n - v;
				
				if (z.sqrMagnitude < 0.01f)
				{
					tracedEdges.Remove(b);
				}
				else
				{
					v = n;
				}
				
				b = c;
				c = b.Next;
			}
		}
	}
	
	private static void OptimizeEdges(float detail)
	{
		if (detail < 1.0f && tracedEdges.Count > 2)
		{
			var a = tracedEdges.First;
			var b = a.Next;
			var c = b.Next;
			
			while (c != tracedEdges.Last)
			{
				var av  = a.Value.Position;
				var bv  = b.Value.Position;
				var cv  = c.Value.Position;
				var ab  = Vector3.Normalize(bv - av);
				var bc  = Vector3.Normalize(cv - bv);
				var abc = Vector3.Dot(ab, bc);
				
				if (abc > detail)
				{
					tracedEdges.Remove(b);
					
					b = c;
					c = c.Next;
				}
				else
				{
					a = b;
					b = c;
					c = c.Next;
				}
			}
		}
	}
	
	private static float GetAlphaOrDefault(int x, int y)
	{
		if (x >= pixelsL && y >= pixelsB && x < pixelsR && y < pixelsT)
		{
			return D2D_AlphaTex.ConvertAlpha(pixels[x + y * pixelsW]);
		}
		
		return default(float);
	}
	
	private static Cell GetCell(int x, int y)
	{
		if (x >= 0 && y >= 0 && x < width && y < height)
		{
			return cells[x + y * width];
		}
		
		return null;
	}
	
	private static Vector2 Transform(float x, float y)
	{
		return new Vector2(x + translation.x, y + translation.y);
	}
}