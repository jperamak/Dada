using UnityEngine;

[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Replace On Trigger")]
public class D2D_ReplaceOnTrigger : MonoBehaviour
{
	public GameObject Spawn;
	
	public bool IgnoreSameTag;
	
	public bool CanHitTrigger;
	
	protected virtual void OnTriggerEnter2D(Collider2D collider)
	{
		if (IgnoreSameTag == true && tag == collider.tag)
		{
			return;
		}
		
		if (CanHitTrigger == false && collider.isTrigger == true)
		{
			return;
		}
		
		Destroy(gameObject);
		
		if (Spawn != null)
		{
			Instantiate(Spawn, transform.position, transform.rotation);
		}
	}
}