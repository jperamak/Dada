using UnityEngine;

[System.Serializable]
public struct D2D_Point
{
	public int X;
	public int Y;
	
	public D2D_Point(int newX, int newY)
	{
		X = newX;
		Y = newY;
	}
	
	public static int DistanceSq(D2D_Point a, D2D_Point b)
	{
		var x = b.X - a.X;
		var y = b.Y - a.Y;
		
		return x * x + y * y;
	}
	
	public static D2D_Point operator + (D2D_Point a, D2D_Point b)
	{
		a.X += b.X;
		a.Y += b.Y;
		
		return a;
	}
	
	public static D2D_Point operator - (D2D_Point a, D2D_Point b)
	{
		a.X -= b.X;
		a.Y -= b.Y;
		
		return a;
	}
	
	public static D2D_Point operator * (D2D_Point a, float b)
	{
		a.X = (int)(a.X * b);
		a.Y = (int)(a.Y * b);
		
		return a;
	}
}