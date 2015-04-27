using UnityEngine;

public abstract class D2D_Collider : MonoBehaviour
{
	public bool IsTrigger;
	
	public PhysicsMaterial2D Material;
	
	private bool dirty;
	
	public void MarkAsDirty()
	{
#if UNITY_EDITOR
		D2D_Helper.SetDirty(this);
#endif
		dirty = true;
	}
	
	public abstract void UpdateColliderSettings();
	
	protected abstract void SetHideFlags(HideFlags hideFlags);
	
	protected abstract void RebuildAll();
	
	protected virtual void Update()
	{
		if (dirty == true)
		{
			dirty = false;
			
			RebuildAll();
		}
	}
	
#if UNITY_EDITOR
	public void SetHideFlags(bool hide)
	{
		var hideFlags = HideFlags.NotEditable;
		
		if (hide == true)
		{
			hideFlags |= HideFlags.HideInInspector;
		}
		
		SetHideFlags(hideFlags);
	}
#endif
}