using UnityEngine;

[RequireComponent(typeof(D2D_Damageable))]
[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Damage On Collision")]
public class D2D_DamageOnCollision : MonoBehaviour
{
	public float DamageScale = 1.0f;
	
	public float DamageThreshold = 1.0f;
	
	private D2D_Damageable damageable;
	
	protected virtual void OnCollisionEnter2D(Collision2D collision)
	{
		var damage = collision.relativeVelocity.magnitude * DamageScale;
		
		if (damage >= DamageThreshold)
		{
			if (damageable == null) damageable = GetComponent<D2D_Damageable>();
			
			damageable.InflictDamage(damage);
		}
	}
}