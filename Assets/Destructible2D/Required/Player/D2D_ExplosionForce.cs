using UnityEngine;

[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Explosion Force")]
public class D2D_ExplosionForce : MonoBehaviour
{
	public LayerMask Layers = -1;
	
	public float Radius = 1.0f;
	
	public float Force = 1.0f;
	
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
			var origin      = transform.position;
			var step        = 360.0f / Samples;
			var scaledForce = Force / Samples;
			
			for (var i = 0; i < Samples; i++)
			{
				var angle     = i * step;
				var direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
				var hit       = Physics2D.Raycast(origin, direction, Radius);
				var collider  = hit.collider;
				
				if (collider != null && collider.isTrigger == false && collider.attachedRigidbody != null)
				{
					var mask = 1 << collider.gameObject.layer;
					
					if ((mask & Layers.value) != 0)
					{
						var force = direction * scaledForce * (1.0f - hit.fraction);
						
						hit.collider.attachedRigidbody.AddForceAtPosition(force, origin);
					}
				}
			}
		}
	}
}