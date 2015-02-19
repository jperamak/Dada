using UnityEngine;
using System.Collections;

public class Aiming : MonoBehaviour {

	private PlayerControl playerCtrl;		// Reference to the PlayerControl script.

	// Use this for initialization
	void Start () 
	{
		playerCtrl = transform.root.GetComponent<PlayerControl>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		float yAxis = playerCtrl.controller.YAxis;
		
		float aimAngle = Mathf.Rad2Deg * Mathf.Asin(yAxis);
		transform.eulerAngles = new Vector3(0, 0, aimAngle);
	}
}
