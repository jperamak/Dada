using UnityEngine;

[RequireComponent(typeof(D2D_DestructibleSprite))]
public abstract class D2D_SpriteCollider : MonoBehaviour
{
	public bool IsTrigger;
	
	public PhysicsMaterial2D Material;
	
	[SerializeField]
	protected GameObject child;
	
	protected D2D_DestructibleSprite destructibleSprite;
	
	protected Texture2D alphaTex;
	
	private bool dirty;
	
	[SerializeField]
	private bool awakeCalled;
	
	public void MarkAsDirty()
	{
#if UNITY_EDITOR
		if (dirty == false)
		{
			D2D_Helper.SetDirty(this);
		}
#endif
		dirty = true;
	}
	
	public void UpdateCollidable()
	{
		if (destructibleSprite == null) destructibleSprite = GetComponent<D2D_DestructibleSprite>();
		
		if (child == null) child = D2D_Helper.CreateGameObject("Colliders", transform);
		
		alphaTex = destructibleSprite.AlphaTex;
		
		var colliderScale  = destructibleSprite.CalculateAlphaTexScale();
		var colliderOffset = destructibleSprite.CalculateAlphaTexOffset();
		
		if (child.transform.localPosition != colliderOffset)
		{
			child.transform.localPosition = colliderOffset;
		}
		
		if (child.transform.localScale != colliderScale)
		{
			child.transform.localScale = colliderScale;
		}
	}
	
	protected virtual void Awake()
	{
		if (awakeCalled == false)
		{
			awakeCalled = true;
			
			RebuildAllColliders();
		}
	}
	
	protected virtual void Update()
	{
		UpdateCollidable();
		
		if (dirty == true)
		{
			dirty = false;
			
			RebuildAllColliders();
		}
	}
	
	protected virtual void OnEnable()
	{
		if (child != null)
		{
			child.SetActive(true);
		}
	}
	
	protected virtual void OnDisable()
	{
		if (child != null)
		{
			child.SetActive(false);
		}
	}
	
	protected virtual void OnDestroy()
	{
		D2D_Helper.DestroyManaged(DestroyChild);
	}
	
	protected virtual void OnAlphaTexReplaced()
	{
		RebuildAllColliders();
	}
	
	protected virtual void OnAlphaTexModified(D2D_Rect rect)
	{
		RebuildColliders(rect.XMin, rect.XMax, rect.YMin, rect.YMax);
	}
	
	public abstract void UpdateColliderSettings();
	
	public abstract void RebuildAllColliders();
	
	public abstract void RebuildColliders(int xMin, int xMax, int yMin, int yMax);
	
	private void DestroyChild()
	{
		child = D2D_Helper.Destroy(child);
	}
}