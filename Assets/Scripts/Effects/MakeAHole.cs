using UnityEngine;

public class MakeAHole : Effect{

	public LayerMask Layers = -1;
	public Texture2D StampTex;
	public float Hardness = 1.0f;
	public Vector2 Size = Vector2.one;


	protected override void Execute (){
	
//		var angle = transform.rotation.eulerAngles.z + AngleOffset + Random.Range(-0.5f, 0.5f) * AngleRandomness;
		
		D2D_Destructible.StampAll(transform.position, Size, 0f, StampTex, Hardness, Layers);
	}
}