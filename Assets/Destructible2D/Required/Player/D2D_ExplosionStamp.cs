using UnityEngine;

[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Explosion Stamp")]
public class D2D_ExplosionStamp : MonoBehaviour
{
	public LayerMask Layers = -1;
	
	public Texture2D StampTex;
	
	public float Hardness = 1.0f;
	
	public Vector2 Size = Vector2.one;
	
	public float AngleOffset;
	
	public float AngleRandomness;
	
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
		var angle = transform.rotation.eulerAngles.z + AngleOffset + Random.Range(-0.5f, 0.5f) * AngleRandomness;
		
		D2D_Destructible.StampAll(transform.position, Size, angle, StampTex, Hardness, Layers);
	}
}