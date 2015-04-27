using UnityEngine;

[AddComponentMenu("Destructible 2D/D2D FPS Counter")]
public class D2D_FpsCounter : MonoBehaviour
{
	public GUIText Text;
	
	private float counter;
	
	private int frames;
	
	private float fps;
	
	protected virtual void Update()
	{
		counter += Time.deltaTime;
		frames  += 1;
		
		if (counter >= 1.0f)
		{
			fps = (float)frames / counter;
			
			counter = 0.0f;
			frames  = 0;
		}
		
		if (Text != null)
		{
			Text.text = "FPS: " + fps.ToString("0");
		}
	}
}