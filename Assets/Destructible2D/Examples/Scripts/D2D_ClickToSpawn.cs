using UnityEngine;

[AddComponentMenu("Destructible 2D/D2D Click To Spawn")]
public class D2D_ClickToSpawn : MonoBehaviour
{
	public GameObject Prefab;
	
	protected virtual void Update()
	{
		if (Input.GetMouseButtonDown(0) == true && Prefab != null && Camera.main != null)
		{
			var ray      = Camera.main.ScreenPointToRay(Input.mousePosition);
			var distance = D2D_Helper.Divide(ray.origin.z, ray.direction.z);
			var point    = ray.origin - ray.direction * distance;
			
			D2D_Helper.CloneGameObject(Prefab, null).transform.position = point;
		}
	}
}