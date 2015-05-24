using UnityEngine;

[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Explosion Damage")]
public class D2D_ExplosionDamage : MonoBehaviour
{
	public LayerMask Layers = -1;
	
	public float Radius = 1.0f;
	
	public float Damage = 10.0f;
	
	public int Samples = 32;
	
	public bool HasExploded;
	
	protected virtual void Update()
	{
		if (HasExploded == false)
		{
			HasExploded = true;
			
			Explode();
		}
	}
	
	public void Explode()
	{
		if (Samples > 0)
		{
			var origin       = transform.position;
			var step         = 360.0f / Samples;
			var scaledDamage = Damage / Samples;
			
			for (var i = 0; i < Samples; i++)
			{
				var angle     = i * step;
				var direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
				var hit       = Physics2D.Raycast(origin, direction, Radius);
				var collider  = hit.collider;
				
				if (collider != null && collider.isTrigger == false)
				{
					var mask = 1 << collider.gameObject.layer;
					
					if ((mask & Layers.value) != 0)
					{
						var damageable = D2D_Helper.GetComponentUpwards<D2D_Damageable>(collider.transform);
						
						if (damageable != null)
						{
							damageable.InflictDamage(scaledDamage);
						}
					}
				}
			}
		}
	}
}