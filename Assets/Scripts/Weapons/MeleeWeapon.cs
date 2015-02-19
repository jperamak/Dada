using UnityEngine;
using System.Collections;

public class MeleeWeapon : MonoBehaviour 
{

	public MeleeStrike meleeStrike;
	public GameObject aiming;
	public float cooldown;
	private PlayerControl playerCtrl;		// Reference to the PlayerControl script.

	// Use this for initialization
	void Awake () 
	{
		playerCtrl = transform.root.GetComponent<PlayerControl>();
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		float angle = aiming.transform.eulerAngles.z; //Mathf.Rad2Deg * Mathf.Asin(y);
		transform.eulerAngles = new Vector3(0, 0, angle);

		if(playerCtrl.controller.GetButtonDown(VirtualKey.MELEE) )// && time > cooldown)
		{
			//time = 0;
			// ... set the animator Shoot trigger parameter and play the audioclip.
			//anim.SetTrigger("Shoot");
			//audio.Play();
			
			Transform spawnPoint = transform.Find("StrikeSpawnPoint");
			
			// If the player is facing right...
		//	if(playerCtrl.facingRight)
		//	{
				
				// ... instantiate the rocket facing right and set it's velocity to the right. 
				MeleeStrike strikeInstance = Instantiate(meleeStrike, spawnPoint.position, Quaternion.Euler(new Vector3(0,0,angle))) as MeleeStrike;
				//strikeInstance.transform.eulerAngles = new Vector3(0, 0, angle);
				//				bulletInstance.velocity = new Vector2(speed, 0);
		//	}
		//	else
		//	{
				// Otherwise instantiate the rocket facing left and set it's velocity to the left.
		//		Rigidbody2D bulletInstance = Instantiate(rocket, spawnPoint.position, Quaternion.Euler(new Vector3(0,0,180f))) as Rigidbody2D;
				//bulletInstance.velocity = new Vector2(-speed, 0);
		//	}
		}
	
	}
}
