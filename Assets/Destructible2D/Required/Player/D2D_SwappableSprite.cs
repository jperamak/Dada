using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(D2D_Damageable))]
[RequireComponent(typeof(SpriteRenderer))]
[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Swappable Sprite")]
public class D2D_SwappableSprite : MonoBehaviour
{
	[System.Serializable]
	public class DamageLevel
	{
		public Sprite Sprite;
		
		public float DamageRequired;
	}
	
	public List<DamageLevel> DamageLevels = new List<DamageLevel>();
	
	private SpriteRenderer spriteRenderer;
	
	private D2D_Damageable damageable;
	
	public DamageLevel AddDamageLevel(Sprite sprite = null, float damageRequired = 0.0f)
	{
		if (DamageLevels == null)
		{
			DamageLevels = new List<DamageLevel>();
		}
		
		var newDamageLevel = new DamageLevel(); DamageLevels.Add(newDamageLevel);
		
		newDamageLevel.Sprite         = sprite;
		newDamageLevel.DamageRequired = damageRequired;
		
		return newDamageLevel;
	}
	
	protected void OnDamageInflicted(float amount)
	{
		UpdateSprite();
	}
	
	public void UpdateSprite()
	{
		if (damageable == null) damageable = GetComponent<D2D_Damageable>();
		
		UpdateDamageLevels();
		
		var bestDamageLevel = default(DamageLevel);
		var damage          = damageable.Damage;
		
		foreach (var damageLevel in DamageLevels)
		{
			if (damageLevel != null)
			{
				if (bestDamageLevel == null || damage >= damageLevel.DamageRequired)
				{
					// Skip if this is below the best
					if (bestDamageLevel != null && damageLevel.DamageRequired < bestDamageLevel.DamageRequired)
					{
						continue;
					}
					
					bestDamageLevel = damageLevel;
				}
			}
		}
		
		// Replace sprite?
		if (bestDamageLevel != null)
		{
			if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
			
			if (spriteRenderer.sprite != bestDamageLevel.Sprite)
			{
				if (bestDamageLevel.Sprite != null)
				{
					spriteRenderer.sprite = bestDamageLevel.Sprite;
				}
				else
				{
					spriteRenderer.sprite = null;
				}
			}
		}
	}
	
	protected virtual void Awake()
	{
		UpdateDamageLevels();
	}
	
#if UNITY_EDITOR
	protected virtual void Reset()
	{
		UpdateDamageLevels();
	}
#endif

#if UNITY_EDITOR
	protected virtual void OnValidate()
	{
		UpdateSprite();
	}
#endif
	
	private void UpdateDamageLevels()
	{
		if (DamageLevels == null)
		{
			DamageLevels = new List<DamageLevel>();
		}
		
		// Copy default damage level from SpriteRenderer?
		if (DamageLevels.Count == 0)
		{
			if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
			
			if (spriteRenderer.sprite != null)
			{
				var newDamageLevel = new DamageLevel(); DamageLevels.Add(newDamageLevel);
				
				newDamageLevel.Sprite = spriteRenderer.sprite;
			}
		}
	}
}