using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu("Destructible 2D/D2D Damageable Sprite")]
[RequireComponent(typeof(SpriteRenderer))]
public class D2D_DamageableSprite : MonoBehaviour
{
	[System.Serializable]
	public class DamageLevel
	{
		public Sprite Sprite;
		
		public float DamageRequired;
	}
	
	public float Damage;
	
	public bool AllowDestruction;
	
	public float DamageLimit;
	
	public GameObject ReplaceWith;
	
	public List<DamageLevel> DamageLevels = new List<DamageLevel>();
	
	private SpriteRenderer spriteRenderer;
	
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
	
	public void InflictDamage(float amount)
	{
		Damage += amount;
		
		UpdateSprite();
	}
	
	public void UpdateSprite()
	{
		var bestDamageLevel = default(DamageLevel);
		
		if (DamageLevels != null)
		{
			foreach (var damageLevel in DamageLevels)
			{
				if (damageLevel != null)
				{
					if (bestDamageLevel == null || Damage >= damageLevel.DamageRequired)
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
		
		UpdateDestruction();
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
	
	private void UpdateDestruction()
	{
		if (AllowDestruction == true)
		{
#if UNITY_EDITOR
			if (Application.isPlaying == false)
			{
				return;
			}
#endif
			if (Damage >= DamageLimit)
			{
				if (ReplaceWith != null)
				{
					D2D_Helper.CloneGameObject(ReplaceWith, transform.parent, transform.position, transform.rotation);
				}
				
				D2D_Helper.Destroy(gameObject);
			}
		}
	}
}