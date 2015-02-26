using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
	public Rigidbody2D Rocket;				// Prefab of the rocket.
	public GameObject Aiming;
	public float Speed = 20f;				// The speed the rocket will fire at.
    public float Cooldown;
	public float Angle = 5f;
	
	public float AimingDegPerSec = 30f;
	public float MinAngle = -80f;
	public float MaxAngle = 90f;


	private PlayerControl _playerCtrl;		// Reference to the PlayerControl script.
	private Animator _anim;					// Reference to the Animator component.
    private float _time;

	void Awake()
	{
		// Setting up the references.
		_anim = transform.root.gameObject.GetComponent<Animator>();
		_playerCtrl = transform.root.GetComponent<PlayerControl>();
        _time = Cooldown;
	}


	void Update ()
	{
        _time += Time.deltaTime;

		/*float h = playerCtrl.controller.YAxis;
        if (Mathf.Abs(h) < 0.2f)
            h = 0;
		angle += h * aimingDegPerSec * Time.deltaTime;
		transform.eulerAngles = new Vector3(0, 0, angle);
		angle = Mathf.Clamp(angle, minAngle, maxAngle);
        */

        //float y = playerCtrl.controller.YAxis;

		Angle = Aiming.transform.eulerAngles.z; //Mathf.Rad2Deg * Mathf.Asin(y);
        transform.eulerAngles = new Vector3(0, 0, Angle);

		if(_playerCtrl.controller.GetButtonDown(VirtualKey.SHOOT) && _time > Cooldown)
		{
            _time = 0;
			// ... set the animator Shoot trigger parameter and play the audioclip.
			_anim.SetTrigger("Shoot");
			audio.Play();

			Transform spawnPoint = transform.Find("RocketSpawnPoint");

			// If the player is facing right...
			if(_playerCtrl.facingRight)
			{
				
				// ... instantiate the rocket facing right and set it's velocity to the right. 
				Rigidbody2D bulletInstance = Instantiate(Rocket, spawnPoint.position, Quaternion.Euler(new Vector3(0,0,0))) as Rigidbody2D;
				//				bulletInstance.velocity = new Vector2(speed, 0);
				bulletInstance.velocity = Quaternion.Euler(0, 0, Angle) * new Vector2(Speed,0);
			}
			else
			{
				// Otherwise instantiate the rocket facing left and set it's velocity to the left.
				Rigidbody2D bulletInstance = Instantiate(Rocket, spawnPoint.position, Quaternion.Euler(new Vector3(0,0,180f))) as Rigidbody2D;
				//bulletInstance.velocity = new Vector2(-speed, 0);
				bulletInstance.velocity = Quaternion.Euler(0, 0, -Angle) * new Vector2(-Speed,0);
			}
		}
	}
}
