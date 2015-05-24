using UnityEngine;

[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Replace On Collision")]
public class D2D_ReplaceOnCollision : MonoBehaviour
{
	public float RelativeVelocityRequired;
	
	public GameObject Spawn;
	
	protected virtual void OnCollisionEnter2D(Collision2D collision)
	{
		Destroy(gameObject);
		
		if (Spawn != null)
		{
			if (collision.relativeVelocity.magnitude >= RelativeVelocityRequired)
			{
				var contact0 = collision.contacts[0];
				
				Instantiate(Spawn, contact0.point, transform.rotation);
			}
		}
	}
}