using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Destructible 2D/D2D Demo GUI")]
public class D2D_DemoGUI : MonoBehaviour
{
	protected virtual void OnGUI()
	{
		var r1 = new Rect(5, 50 + 55 * 0, 100, 50);
		var r2 = new Rect(5, 50 + 55 * 1, 100, 50);
		var r3 = new Rect(5, 50 + 55 * 2, 100, 50);
		var r4 = new Rect(5, 50 + 55 * 3, 100, 50);
		
		if (GUI.Button(r1, "Reload") == true)
		{
			Application.LoadLevel(Application.loadedLevel);
		}
		
		if (GUI.Button(r2, "Halve") == true)
		{
			foreach (var destructibleSprite in D2D_DestructibleSprite.DestructibleSprites)
			{
				destructibleSprite.HalveAlphaTexAndSplitMinPixels();
			}
		}
		
		if (GUI.Button(r3, "Blur") == true)
		{
			foreach (var destructibleSprite in D2D_DestructibleSprite.DestructibleSprites)
			{
				destructibleSprite.BlurAlphaTex();
			}
		}
		
		if (GUI.Button(r4, "Sharpness") == true)
		{
			foreach (var destructibleSprite in D2D_DestructibleSprite.DestructibleSprites)
			{
				destructibleSprite.Sharpness *= 2;
			}
		}
	}
}