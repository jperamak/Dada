using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(D2D_DestructibleSprite))]
[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Detachable Sprite")]
public class D2D_DetachableSprite : MonoBehaviour
{
	public D2D_SpriteCollider OldCollider;
	
	public D2D_SpriteCollider NewCollider;
	
	private new Rigidbody2D rigidbody2D;
	
	[System.NonSerialized]
	private bool detached;
	
	[System.NonSerialized]
	private List<D2D_Fixture> fixtures = new List<D2D_Fixture>();
	
	private bool dirty;
	
	protected virtual void OnAlphaTexReplaced()
	{
#if UNITY_EDITOR
		if (Application.isPlaying == false)
		{
			return;
		}
#endif
		CheckFixtures();
	}
	
	protected virtual void OnAlphaTexModified(D2D_Rect rect)
	{
#if UNITY_EDITOR
		if (Application.isPlaying == false)
		{
			return;
		}
#endif
		CheckFixtures();
	}
	
	protected virtual void Update()
	{
#if UNITY_EDITOR
		if (Application.isPlaying == false)
		{
			UpdateState();
			
			return;
		}
#endif
		CheckFixtures();
	}
	
	private void UpdateState()
	{
		if (rigidbody2D == null) rigidbody2D = GetComponent<Rigidbody2D>();
		
		if (detached == false)
		{
			if (rigidbody2D.isKinematic == false) rigidbody2D.isKinematic = true;
			
			if (OldCollider != null) OldCollider.enabled = true;
			
			if (NewCollider != null) NewCollider.enabled = false;
		}
	}
	
	private void CheckFixtures()
	{
		UpdateState();
		
		fixtures.RemoveAll(f => f == null); // Note: should go first, as the fixture list may change at runtime
		
		// Repopulate if it appears clear, as the list may be outdated
		if (fixtures.Find(f => f.Pinned == true) == null)
		{
			fixtures.Clear();
			fixtures.AddRange(GetComponentsInChildren<D2D_Fixture>());
		}
		
		// If we're sure there are no pinned fixtures left, detach!
		if (fixtures.Find(f => f.Pinned == true) == null)
		{
			D2D_Helper.Destroy(OldCollider);
			
			if (NewCollider != null) NewCollider.enabled = true;
			
			if (rigidbody2D.isKinematic == true) rigidbody2D.isKinematic = false;
			
			D2D_Helper.Destroy(this);
		}
	}
}