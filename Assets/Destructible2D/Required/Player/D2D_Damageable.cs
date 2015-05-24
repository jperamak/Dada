using UnityEngine;
using System.Collections;

[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Damageable")]
public class D2D_Damageable : MonoBehaviour
{
	public float Damage;
	
	public float Age;
	
	public float ActivateDelay = 0.5f;
	
	public bool AllowDestruction;
	
	public float DamageLimit = 100.0f;
	
	public GameObject ReplaceWith;
	
	public void InflictDamage(float amount)
	{
		if (Age >= ActivateDelay) // Discard damage until it's old enough
		{
			if (amount != 0.0f)
			{
				Damage += amount;
				
				D2D_Helper.BroadcastMessage(transform, "OnDamageInflicted", amount, SendMessageOptions.DontRequireReceiver);
				
				UpdateDestruction();
			}
		}
	}
	
	public void UpdateDestruction()
	{
		if (AllowDestruction == true)
		{
#if UNITY_EDITOR
			if (Application.isPlaying == false)
			{
				return;
			}
#endif
			if (Damage >= DamageLimit)
			{
				if (ReplaceWith != null)
				{
					D2D_Helper.CloneGameObject(ReplaceWith, transform.parent, transform.position, transform.rotation);
				}
				
				D2D_Helper.Destroy(gameObject);
			}
		}
	}
	
	protected virtual void Update()
	{
		Age += Time.deltaTime;
	}
	
	protected virtual void OnDestructibleSplit(D2D_SplitData splitData)
	{
		Age    = 0.0f; // Reset age if this is a split part
		Damage = 0.0f; // Reset damage if this is a split part
	}
}