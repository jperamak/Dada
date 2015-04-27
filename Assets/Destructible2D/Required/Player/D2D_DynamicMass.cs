using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(D2D_DestructibleSprite))]
[RequireComponent(typeof(Rigidbody2D))]
[AddComponentMenu("Destructible 2D/D2D Dynamic Mass")]
public class D2D_DynamicMass : MonoBehaviour
{
	public float MassPerPixel = 0.01f;
	
	private D2D_DestructibleSprite destructibleSprite;
	
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
		
		var newMass = destructibleSprite.SolidPixelCount * MassPerPixel;
		
		if (GetComponent<Rigidbody2D>().mass != newMass)
		{
			GetComponent<Rigidbody2D>().mass = newMass;
		}
	}
}