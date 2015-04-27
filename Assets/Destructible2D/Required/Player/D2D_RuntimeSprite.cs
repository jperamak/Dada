using UnityEngine;

public class D2D_RuntimeSprite : MonoBehaviour
{
	public Sprite Sprite;
	
	protected virtual void OnDestroy()
	{
		D2D_Helper.Destroy(Sprite);
	}
}