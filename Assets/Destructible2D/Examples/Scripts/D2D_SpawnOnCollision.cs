using UnityEngine;

[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Spawn On Collision")]
public class D2D_SpawnOnCollision : MonoBehaviour
{
	public float RelativeVelocityRequired = 1.0f;
	
	public GameObject Spawn;
	
	public float SpawnCooldown;
	
	private float cooldownTimer;
	
	protected virtual void OnCollisionEnter2D(Collision2D collision)
	{
		if (Spawn != null && cooldownTimer <= 0.0f && collision.relativeVelocity.magnitude >= RelativeVelocityRequired)
		{
			cooldownTimer = SpawnCooldown;
			
			var contact0 = collision.contacts[0];
			
			Instantiate(Spawn, contact0.point, transform.rotation);
		}
	}
	
	protected virtual void Update()
	{
		cooldownTimer -= Time.deltaTime;
	}
}