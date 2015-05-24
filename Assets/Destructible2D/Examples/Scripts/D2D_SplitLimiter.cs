using UnityEngine;

// This component will disable destruction, splitting, or fracturing, if there are either too few pixels in the destructible sprite, or its depth is too high
[RequireComponent(typeof(D2D_Destructible))]
[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Split Limiter")]
public class D2D_SplitLimiter : MonoBehaviour
{
	// If your D2D_DestructibleSprite has less than this amount of pixels, its Indestructible setting will be enabled
	public int MinPixelsForDestructible = 100;
	
	// If your D2D_DestructibleSprite has less than this amount of pixels, your D2D_Splittable component will be destroyed
	public int MinPixelsForSplitting = 100;
	
	// If your D2D_DestructibleSprite has less than this amount of pixels, your D2D_QuadFracturer component will be destroyed
	public int MinPixelsForFracturing = 100;
	
	// If your D2D_DestructibleSprite has been split more than this amount, its Indestructible setting will be enabled
	public int MaxDepthForDestructible = 10;
	
	// If your D2D_DestructibleSprite has been split more than this amount, your D2D_Splittable component will be destroyed
	public int MaxDepthForSplitting = 10;
	
	// If your D2D_DestructibleSprite has been split more than this amount, your D2D_QuadFracturer component will be destroyed
	public int MaxDepthForFracturing = 10;
	
	private D2D_Destructible destructible;
	
	private D2D_Splittable splittable;
	
	private D2D_Fracturer fracturer;
	
	protected virtual void Update()
	{
		if (destructible == null) destructible = GetComponent<D2D_Destructible>();
		if (splittable   == null) splittable   = GetComponent<D2D_Splittable>();
		if (fracturer    == null) fracturer    = GetComponent<D2D_Fracturer>();
		
		var splitDepth = destructible.SplitDepth;
		var pixelCount = destructible.SolidPixelCount;
		
		if (pixelCount < MinPixelsForDestructible || splitDepth > MaxDepthForDestructible)
		{
			destructible.Indestructible = true;
		}
		
		if (splittable != null)
		{
			if (pixelCount < MinPixelsForSplitting || splitDepth > MaxDepthForSplitting)
			{
				D2D_Helper.Destroy(splittable);
			}
		}
		
		if (fracturer != null)
		{
			if (pixelCount < MinPixelsForFracturing || splitDepth > MaxDepthForFracturing)
			{
				D2D_Helper.Destroy(fracturer);
			}
		}
	}
}