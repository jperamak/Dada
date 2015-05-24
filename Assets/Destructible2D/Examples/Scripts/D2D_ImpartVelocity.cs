using UnityEngine;

// When a rigidbody sprite is split, it loses its velocity values. This script allows them to persist.
[RequireComponent(typeof(Rigidbody2D))]
[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Impart Velocity")]
public class D2D_ImpartVelocity : MonoBehaviour
{
	public Vector3 Velocity;
	
	public float AngularVelocity;
	
	private Rigidbody2D rigidbody2d;
	
	protected virtual void FixedUpdate()
	{
		if (rigidbody2d == null) rigidbody2d = GetComponent<Rigidbody2D>();
		
		Velocity        = rigidbody2d.velocity;
		AngularVelocity = rigidbody2d.angularVelocity;
	}
	
	protected virtual void OnDestructibleSplit(D2D_SplitData splitData)
	{
		if (rigidbody2d == null) rigidbody2d = GetComponent<Rigidbody2D>();
		
		rigidbody2d.velocity        = Velocity;
		rigidbody2d.angularVelocity = AngularVelocity;
	}
}