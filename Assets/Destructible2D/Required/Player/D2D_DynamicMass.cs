using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(D2D_DestructibleSprite))]
[RequireComponent(typeof(Rigidbody2D))]
[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Dynamic Mass")]
public class D2D_DynamicMass : MonoBehaviour
{
	public float MassPerPixel = 0.01f;
	
	private D2D_DestructibleSprite destructibleSprite;
	
	private new Rigidbody2D rigidbody2D;
	
	protected virtual void FixedUpdate()
	{
		UpdateMass();
	}
	
	protected virtual void LateUpdate()
	{
		UpdateMass();
	}
	
	private void UpdateMass()
	{
		if (destructibleSprite == null) destructibleSprite = GetComponent<D2D_DestructibleSprite>();
		
		if (rigidbody2D == null) rigidbody2D = GetComponent<Rigidbody2D>();
		
		var newMass = destructibleSprite.SolidPixelCount * MassPerPixel;
		
		if (rigidbody2D.mass != newMass)
		{
			rigidbody2D.mass = newMass;
		}
	}
}