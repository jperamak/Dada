using UnityEngine;
using System.Collections.Generic;

public static class D2D_EdgeCalculator
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
	
	private static int cellCount;
	
	private static List<Cell> cells = new List<Cell>();
	
	private static List<EdgeCollider2D> generatedEdgeColliders = new List<EdgeCollider2D>();
	
	private static List<Vector2> generatedPoints = new List<Vector2>();
	
	private static int cellPointCount;
	
	private static List<Point> cellPoints = new List<Point>();
	
	private static LinkedList<Point> tracedEdges = new LinkedList<Point>();
	
	private static Vector2 translation;
	
	private static Color[] pixels;
	
	private static int pixelsX;
	
	private static int pixelsY;
	
	private static int pixelsWidth;
	
	private static int pixelsHeight;
	
	private static void GetPixels(Texture2D alphaTex, int l, int b, int r, int t)
	{
		l = Mathf.Max(l, 0);
		b = Mathf.Max(b, 0);
		r = Mathf.Min(r, alphaTex.width);
		t = Mathf.Min(t, alphaTex.height);
		
		pixelsX      = l;
		pixelsY      = b;
		pixelsWidth  = r - l;
		pixelsHeight = t - b;
		pixels       = alphaTex.GetPixels(pixelsX, pixelsY, pixelsWidth, pixelsHeight);
	}
	
	public static List<EdgeCollider2D> Generate(GameObject gameObject, Texture2D alphaTex, int newXMin, int newXMax, int newYMin, int newYMax, float tolerance)
	{
		cellCount      = 0;
		cellPointCount = 0;
		
		generatedEdgeColliders.Clear();
		
		if (gameObject != null && alphaTex != null)
		{
			xMin   = newXMin; if (xMin == 0) xMin -= 1; // Expand left?
			yMin   = newYMin; if (yMin == 0) yMin -= 1; // Expand bottom?
			xMax   = newXMax;
			yMax   = newYMax;
			width  = xMax - xMin;
			height = yMax - yMin;
			
			GetPixels(alphaTex, newXMin - 1, newYMin - 1, newXMax + 1, newYMax + 1);
			
			if (width > 0 && height > 0)
			{
				if (cells.Capacity < width * height)
				{
					cells.Capacity = width * height;
				}
				
				for (var y = 0; y < height; y++)
				{
					for (var x = 0; x < width; x++)
					{
						var cell = GetNextCell();
						var bl   = GetAlphaOrDefault(xMin + x    , yMin + y    );
						var br   = GetAlphaOrDefault(xMin + x + 1, yMin + y    );
						var tl   = GetAlphaOrDefault(xMin + x    , yMin + y + 1);
						var tr   = GetAlphaOrDefault(xMin + x + 1, yMin + y + 1);
						
						cell.X = x;
						cell.Y = y;
						
						translation.x = xMin + x + 0.5f;
						translation.y = yMin + y + 0.5f;
						
						CalculateCell(cell, bl, br, tl, tr, 0.5f);
					}
				}
				
				for (var i = 0; i < cellCount; i++)
				{
					var cell = cells[i];
					
					for (var j = 0; j < cell.PointCount; j++)
					{
						var point = cellPoints[cell.PointIndex + j];
						
						if (point.Used == false)
						{
							TraceEdges(cell, point);
							
							OptimizeEdges(tolerance);
							
							CompileTracedEdges(gameObject);
						}
					}
				}
				
				/*
				var border = gameObject.AddComponent<EdgeCollider2D>(); generatedEdgeColliders.Add(border);
				
				border.points = new Vector2[] {
					new Vector2(xMin, yMin),
					new Vector2(xMax, yMin),
					new Vector2(xMax, yMax),
					new Vector2(xMin, yMax),
					new Vector2(xMin, yMin) };*/
					
			}
		}
		
		return generatedEdgeColliders;
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
	
	private static void CompileTracedEdges(GameObject gameObject)
	{
		var edgeCollider2D = gameObject.AddComponent<EdgeCollider2D>(); generatedEdgeColliders.Add(edgeCollider2D);
		
		generatedPoints.Clear();
		
		foreach (var point in tracedEdges)
		{
			generatedPoints.Add(point.Position);
		}
		
		edgeCollider2D.points = generatedPoints.ToArray();
	}
	
	private static void CalculateCell(Cell cell, float bl, float br, float tl, float tr, float threshold)
	{
		var index = cellPointCount;
		var count = 0;
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
				if (useTl == true && useBr == true)
				{
					cellPoints[index + 0].Other = cellPoints[index + 3];
					cellPoints[index + 1].Other = cellPoints[index + 2];
					cellPoints[index + 2].Other = cellPoints[index + 1];
					cellPoints[index + 3].Other = cellPoints[index + 0];
				}
				else
				{
					cellPoints[index + 0].Other = cellPoints[index + 1];
					cellPoints[index + 1].Other = cellPoints[index + 0];
					cellPoints[index + 2].Other = cellPoints[index + 3];
					cellPoints[index + 3].Other = cellPoints[index + 2];
				}
			}
			break;
		}
		
		cell.PointIndex = index;
		cell.PointCount = count;
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
	
	private static void OptimizeEdges(float tolerance)
	{
		if (tolerance > 0.0f && tracedEdges.Count > 2)
		{
			var a = tracedEdges.First;
			var b = a.Next;
			var c = b.Next;
			
			while (true)
			{
				var ab  = Vector3.Normalize(b.Value.Position - a.Value.Position);
				var bc  = Vector3.Normalize(c.Value.Position - b.Value.Position);
				var abc = Vector3.Dot(ab, bc);
				
				if (abc > (1.0f - tolerance))
				{
					tracedEdges.Remove(b);
					
					if (c != tracedEdges.Last)
					{
						b = c;
						c = c.Next;
					}
					else
					{
						return;
					}
				}
				else
				{
					if (c != tracedEdges.Last)
					{
						a = b;
						b = c;
						c = c.Next;
					}
					else
					{
						return;
					}
				}
			}
		}
	}
	
	private static float GetAlphaOrDefault(int x, int y)
	{
		x -= pixelsX;
		y -= pixelsY;
		
		if (x >= 0 && y >= 0 && x < pixelsWidth && y < pixelsHeight)
		{
			return pixels[x + y * pixelsWidth].a;
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
		x = x + translation.x;
		y = y + translation.y;
		
		return new Vector2(x, y);
	}
}