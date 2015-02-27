using UnityEngine;
using System.Collections;

public class MeleeWeapon : MonoBehaviour 
{

	public MeleeStrike MeleeStrike;
	public GameObject Aiming;
	public float Cooldown;
	private PlayerControl _playerCtrl;		// Reference to the PlayerControl script.
    private float _time;

	// Use this for initialization
	void Awake () 
	{
		_playerCtrl = transform.root.GetComponent<PlayerControl>();
        _time = Cooldown;
	}
	
	// Update is called once per frame
	void Update () 
	{
        _time += Time.deltaTime;
		float angle = Aiming.transform.eulerAngles.z; //Mathf.Rad2Deg * Mathf.Asin(y);
		transform.eulerAngles = new Vector3(0, 0, angle);

		if(_playerCtrl.controller.GetButtonDown(VirtualKey.MELEE)  && _time > Cooldown)
		{
			_time = 0;
			// ... set the animator Shoot trigger parameter and play the audioclip.
			//anim.SetTrigger("Melee");
			//audio.Play();
			
			Transform spawnPoint = transform.Find("StrikeSpawnPoint");
			
			MeleeStrike strikeInstance = Instantiate(MeleeStrike, spawnPoint.position, Quaternion.Euler(new Vector3(0,0,angle))) as MeleeStrike;
            strikeInstance.transform.parent = _playerCtrl.transform;
            strikeInstance.GetComponent<MeleeStrike>().Player = _playerCtrl;
		}
	
	}
}
