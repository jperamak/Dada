using UnityEngine;

[AddComponentMenu("Destructible 2D/D2D Damage On Collision")]
public class D2D_DamageOnCollision : MonoBehaviour
{
	public D2D_DamageableSprite DamageableSprite;
	
	public float RelativeVelocityRequired = 1.0f;
	
	public float DamageScale = 1.0f;
	
	protected virtual void Awake()
	{
		if (DamageableSprite == null)
		{
			DamageableSprite = GetComponent<D2D_DamageableSprite>();
		}
	}
	
	protected virtual void OnCollisionEnter2D(Collision2D collision)
	{
		if (DamageableSprite != null)
		{
			var magnitude = collision.relativeVelocity.magnitude;
			
			if (magnitude >= RelativeVelocityRequired)
			{
				var damage = (magnitude - RelativeVelocityRequired) * DamageScale;
				
				DamageableSprite.InflictDamage(damage);
			}
		}
	}
	
#if UNITY_EDITOR
	protected virtual void Reset()
	{
		if (DamageableSprite == null)
		{
			DamageableSprite = GetComponent<D2D_DamageableSprite>();
		}
	}
#endif
}