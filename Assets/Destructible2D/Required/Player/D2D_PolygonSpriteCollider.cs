using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Polygon Sprite Collider")]
public class D2D_PolygonSpriteCollider : D2D_SpriteCollider
{
	[System.Serializable]
	public class Cell
	{
		public PolygonCollider2D Collider;
		
		public void Destroy()
		{
			if (Collider != null)
			{
				Collider = D2D_Helper.Destroy(Collider);
			}
		}
		
		public void UpdateColliderSettings(bool isTrigger, PhysicsMaterial2D material)
		{
			if (Collider != null)
			{
				Collider.isTrigger      = isTrigger;
				Collider.sharedMaterial = material;
			}
		}
		
		public void Trim(int pathCount)
		{
			if (pathCount > 0)
			{
				if (Collider.pathCount > pathCount)
				{
					Collider.pathCount = pathCount;
				}
			}
			else
			{
				Destroy();
			}
		}
	}
	
	[D2D_PopupAttribute(8, 16, 32, 64, 128, 256)]
	public int CellSize = 64;
	
	[D2D_RangeAttribute(0.5f, 1.0f)]
	public float Detail = 1.0f;
	
	public bool Binary;
	
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
	
	public override void RebuildAllColliders()
	{
		UpdateCollidable();
		
		if (alphaTex != null)
		{
			RebuildColliders(0, alphaTex.width, 0, alphaTex.height);
		}
		else
		{
			DestroyAllCells();
		}
	}
	
	public override void RebuildColliders(int xMin, int xMax, int yMin, int yMax)
	{
		UpdateCollidable();
		
		UpdateColliders();
		
		if (cells.Count > 0)
		{
			xMin = Mathf.Clamp(xMin - 1, 0, alphaTex.width  - 1);
			yMin = Mathf.Clamp(yMin - 1, 0, alphaTex.height - 1);
			
			var cellXMin = xMin / CellSize;
			var cellYMin = yMin / CellSize;
			var cellXMax = (xMax + 1) / CellSize;
			var cellYMax = (yMax + 1) / CellSize;
			
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
						
						var cell = cells[cellX + cellY * cellsX];
						
						D2D_ColliderBuilder.Calculate(destructibleSprite, xMin, xMax, yMin, yMax, false, Binary);
						
						D2D_ColliderBuilder.Build(child, cell, Detail);
						
						cell.UpdateColliderSettings(IsTrigger, Material);
					}
				}
			}
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
	
	private void UpdateColliders()
	{
		if (alphaTex != null && alphaTex.width > 0 && alphaTex.height > 0 && CellSize > 0 && Detail >= 0.0f)
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
	
	private void DestroyAllCells()
	{
		if (cells.Count > 0)
		{
			foreach (var cell in cells)
			{
				cell.Destroy();
			}
			
			cells.Clear();
			
#if UNITY_EDITOR
			D2D_Helper.SetDirty(this);
#endif
		}
	}
}