using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu("Destructible 2D/D2D Edge Colliders")]
public class D2D_EdgeColliders : D2D_Collider
{
	[System.Serializable]
	public class Cell
	{
		public List<EdgeCollider2D> EdgeCollider2Ds = new List<EdgeCollider2D>();
		
		public void Destroy()
		{
			foreach (var edgeCollider2D in EdgeCollider2Ds)
			{
				D2D_Helper.Destroy(edgeCollider2D);
			}
			
			EdgeCollider2Ds.Clear();
		}
		
		public void ReplaceColliders(List<EdgeCollider2D> newEdgeCollider2Ds)
		{
			Destroy();
			
			EdgeCollider2Ds.AddRange(newEdgeCollider2Ds);
		}
		
		public void UpdateColliderSettings(bool isTrigger, PhysicsMaterial2D material)
		{
			foreach (var edgeCollider2D in EdgeCollider2Ds)
			{
				if (edgeCollider2D != null)
				{
					edgeCollider2D.isTrigger      = isTrigger;
					edgeCollider2D.sharedMaterial = material;
				}
			}
		}
	}
	
	[D2D_PopupAttribute(8, 16, 32, 64, 128, 256)]
	public int CellSize = 64;
	
	[D2D_RangeAttribute(0.0f, 0.3f)]
	public float Tolerance = 0.01f;
	
	[SerializeField]
	private List<Cell> cells = new List<Cell>();
	
	[SerializeField]
	private int cellsX;
	
	[SerializeField]
	private int cellsY;
	
	[SerializeField]
	private int cellsXY;
	
	[SerializeField]
	private int width;
	
	[SerializeField]
	private int height;
	
	public void RebuildAllColliders(Texture2D alphaTex)
	{
		if (alphaTex != null)
		{
			RebuildColliders(alphaTex, 0, alphaTex.width, 0, alphaTex.height);
		}
		else
		{
			DestroyAllCells();
		}
	}
	
	public void RebuildColliders(Texture2D alphaTex, int xMin, int xMax, int yMin, int yMax)
	{
		UpdateColliders(alphaTex);
		
		if (cells.Count > 0)
		{
			//var stopwatch = System.Diagnostics.Stopwatch.StartNew();
			
			xMin = Mathf.Clamp(xMin - 1, 0, alphaTex.width  - 1);
			yMin = Mathf.Clamp(yMin - 1, 0, alphaTex.height - 1);
			
			var cellXMin = xMin / CellSize;
			var cellYMin = yMin / CellSize;
			var cellXMax = (xMax + CellSize - 1) / CellSize;
			var cellYMax = (yMax + CellSize - 1) / CellSize;
			
			for (var cellY = cellYMin; cellY <= cellYMax; cellY++)
			{
				for (var cellX = cellXMin; cellX <= cellXMax; cellX++)
				{
					if (cellX >= 0 && cellX < cellsX && cellY >= 0 && cellY < cellsY)
					{
						xMin = CellSize * cellX;
						yMin = CellSize * cellY;
						xMax = Mathf.Min(CellSize + xMin, alphaTex.width);
						yMax = Mathf.Min(CellSize + yMin, alphaTex.height);
						
						var cell               = cells[cellX + cellY * cellsX];
						var newEdgeCollider2Ds = D2D_EdgeCalculator.Generate(gameObject, alphaTex, xMin, xMax, yMin, yMax, Tolerance);
						
						cell.ReplaceColliders(newEdgeCollider2Ds);
						cell.UpdateColliderSettings(IsTrigger, Material);
					}
				}
			}
			
			//stopwatch.Stop(); Debug.Log("RebuildColliders took " + stopwatch.ElapsedMilliseconds + " ms");
		}
	}
	
	public override void UpdateColliderSettings()
	{
		foreach (var cell in cells)
		{
			if (cell != null)
			{
				cell.UpdateColliderSettings(IsTrigger, Material);
			}
		}
	}
	
	private void UpdateColliders(Texture2D alphaTex)
	{
		if (alphaTex != null && alphaTex.width > 0 && alphaTex.height > 0 && CellSize > 0 && Tolerance >= 0.0f)
		{
			cellsX  = (alphaTex.width  + CellSize - 1) / CellSize;
			cellsY  = (alphaTex.height + CellSize - 1) / CellSize;
			cellsXY = cellsX * cellsY;
			
			if (cells.Count > 0)
			{
				if (cells.Count != cellsXY)
				{
					DestroyAllCells();
				}
				
				if (alphaTex.width != width || alphaTex.height != height)
				{
					DestroyAllCells();
				}
			}
			
			// Rebuild all cells?
			if (cells.Count == 0 && cellsXY > 0)
			{
				width  = alphaTex.width;
				height = alphaTex.height;
				
				for (var i = 0; i < cellsXY; i++)
				{
					cells.Add(new Cell());
				}
			}
		}
		else
		{
			DestroyAllCells();
		}
	}
	
	protected virtual void OnDestroy()
	{
		D2D_Helper.DestroyManaged(DestroyAllCells);
	}
	
	protected override void RebuildAll()
	{
		var destructible = D2D_Helper.GetComponentUpwards<D2D_Destructible>(transform);
		
		if (destructible != null)
		{
			RebuildAllColliders(destructible.AlphaTex);
		}
	}
	
	private void DestroyAllCells()
	{
		if (cells.Count > 0)
		{
			foreach (var cell in cells)
			{
				cell.Destroy();
			}
			
			cells.Clear();
		}
	}
	
#if UNITY_EDITOR
	protected override void SetHideFlags(HideFlags hideFlags)
	{
		foreach (var cell in cells)
		{
			foreach (var edgeCollider2D in cell.EdgeCollider2Ds)
			{
				if (edgeCollider2D != null)
				{
					edgeCollider2D.hideFlags = hideFlags;
				}
			}
		}
	}
#endif
}