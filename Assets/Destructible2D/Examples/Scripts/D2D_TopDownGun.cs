using UnityEngine;

[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Top Down Gun")]
public class D2D_TopDownGun : MonoBehaviour
{
	public GameObject Bullet;
	
	public float ShotCooldown = 0.1f;
	
	public float ShootSpeed = 10.0f;
	
	public bool IsFiring;
	
	private float cooldownTimer;
	
	protected virtual void FixedUpdate()
	{
		if (IsFiring == true && Bullet != null && cooldownTimer <= 0.0f)
		{
			cooldownTimer = ShotCooldown;
			
			var clone = D2D_Helper.CloneGameObject(Bullet, null, transform.position, transform.rotation);
			var body  = clone.GetComponent<Rigidbody2D>();
			
			if (body != null)
			{
				body.velocity = transform.up * ShootSpeed;
			}
		}
	}
	
	protected virtual void Update()
	{
		cooldownTimer -= Time.deltaTime;
	}
}