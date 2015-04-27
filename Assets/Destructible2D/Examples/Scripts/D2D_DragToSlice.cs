using UnityEngine;

[AddComponentMenu("Destructible 2D/D2D Drag To Slice")]
public class D2D_DragToSlice : MonoBehaviour
{
	public Texture2D StampTex;
	
	public float Thickness = 1.0f;
	
	public float Hardness = 1.0f;
	
	public SpriteRenderer Indicator;
	
	private bool down;
	
	private Vector3 startMousePosition;
	
	protected virtual void Update()
	{
		if (Input.GetMouseButton(0) == true && down == false)
		{
			down               = true;
			startMousePosition = Input.mousePosition;
		}
		
		if (Input.GetMouseButton(0) == false && down == true)
		{
			down = false;
			
			if (Camera.main != null)
			{
				var endMousePosition = Input.mousePosition;
				var startPos         = Camera.main.ScreenToWorldPoint(startMousePosition);
				var endPos           = Camera.main.ScreenToWorldPoint(  endMousePosition);
				
				D2D_Destructible.SliceAll(startPos, endPos, Thickness, StampTex, Hardness);
			}
		}
		
		if (Indicator != null)
		{
			Indicator.enabled = down;
			
			if (Camera.main != null && down == true)
			{
				var currentMousePosition = Input.mousePosition;
				var startPos             = Camera.main.ScreenToWorldPoint(  startMousePosition);
				var currentPos           = Camera.main.ScreenToWorldPoint(currentMousePosition);
				var scale                = Vector3.Distance(currentPos, startPos);
				var angle                = D2D_Helper.Atan2(currentPos - startPos) * Mathf.Rad2Deg;
				var newPosition          = Camera.main.ScreenToWorldPoint(startMousePosition);
				
				newPosition.z = Indicator.transform.position.z;
				
				Indicator.transform.position      = newPosition;
				Indicator.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -angle);
				Indicator.transform.localScale    = new Vector3(Thickness, scale, scale);
			}
		}
	}
}