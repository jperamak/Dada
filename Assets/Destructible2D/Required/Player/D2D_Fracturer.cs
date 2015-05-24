using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(D2D_Damageable))]
[RequireComponent(typeof(D2D_Destructible))]
public abstract class D2D_Fracturer : MonoBehaviour
{
	public float DamageLimit = 100.0f;
	
	public int Count = 6;
	
	public static bool BusyFracturing;
	
	protected D2D_Destructible destructible;
	
	protected D2D_Damageable damageable;
	
	protected static Texture2D alphaTex;
	
	protected static int width;
	
	protected static int height;
	
	protected static int total;
	
	[ContextMenu("Fracture")]
	public void Fracture()
	{
		if (destructible == null) destructible = GetComponent<D2D_Destructible>();
		
		alphaTex = destructible.AlphaTex;
		width    = alphaTex.width;
		height   = alphaTex.height;
		total    = width * height;
		
		D2D_SplitBuilder.BeginSplitting(destructible);
		{
			DoFracture();
		}
		D2D_SplitBuilder.EndSplitting(D2D_SplitOrder.Default);
	}
	
	protected virtual void OnDestructibleValidSplit(D2D_SplitData splitData)
	{
		DamageLimit /= 2;
		Count       /= 2;
		
		if (Count <= 0)
		{
			D2D_Helper.Destroy(this);
		}
	}
	
	protected void OnDamageInflicted(float amount)
	{
		UpdateFracture();
	}
	
	protected abstract void DoFracture();
	
	private void UpdateFracture()
	{
#if UNITY_EDITOR
		if (Application.isPlaying == false)
		{
			return;
		}
#endif
		if (damageable == null) damageable = GetComponent<D2D_Damageable>();
		
		if (damageable.Damage >= DamageLimit)
		{
			Fracture();
		}
	}
}