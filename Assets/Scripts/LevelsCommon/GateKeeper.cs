using UnityEngine;
using System.Collections;

public class GateKeeper : MonoBehaviour {

	public int ownedByTeamNumber;

	private GameObject _portcullis;


	// Use this for initialization
	void Start () {
		_portcullis = transform.parent.FindChild("Portcullis").gameObject;

		/*
		if (ownedByTeamNumber == 0)
			_portcullis.GetComponent<SpriteRenderer>().color = Color.red;
		else if (ownedByTeamNumber == 1)
			_portcullis.GetComponent<SpriteRenderer>().color = Color.blue;
		else if (ownedByTeamNumber == 2)
			_portcullis.GetComponent<SpriteRenderer>().color = Color.green;
		else if (ownedByTeamNumber == 3)
			_portcullis.GetComponent<SpriteRenderer>().color = Color.yellow;
*/
	}


	//BUG: IF THERE ARE TWO TEAM MEMBERS IN AREA THEN THIS WONT WORK PROPERBLY
	void OnTriggerEnter2D(Collider2D col){
		Hero hero = col.gameObject.GetComponent<Hero>();

		if (hero == null)
			return;

		if (hero.PlayerInstance.TeamNumber == ownedByTeamNumber)
			_portcullis.SetActive(false);

	}

	void OnTriggerExit2D(Collider2D col){
		Hero hero = col.gameObject.GetComponent<Hero>();
	
		if (hero == null)
			return;
	
		if (hero.PlayerInstance.TeamNumber == ownedByTeamNumber)
			_portcullis.SetActive(true);
	
	}




}
