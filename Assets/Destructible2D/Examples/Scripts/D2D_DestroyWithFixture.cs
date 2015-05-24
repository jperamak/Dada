using UnityEngine;

[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Destroy With Fixture")]
public class D2D_DestroyWithFixture : MonoBehaviour
{
	public D2D_Fixture RequiredFixture;
	
	protected virtual void Update()
	{
		if (RequiredFixture == null)
		{
			D2D_Helper.Destroy(gameObject);
		}
	}
}