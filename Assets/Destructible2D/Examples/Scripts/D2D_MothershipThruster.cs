using UnityEngine;

[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Mothership Thruster")]
public class D2D_MothershipThruster : MonoBehaviour
{
	public ParticleSystem Particles;
	
	public D2D_MothershipCore Core;
	
	protected virtual void Update()
	{
		// Core not connected?
		if (Particles != null && Core == null)
		{
			Particles.enableEmission = false;
		}
	}
}