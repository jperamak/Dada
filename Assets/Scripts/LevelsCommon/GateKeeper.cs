using UnityEngine;
using System.Collections;

public class GateKeeper : MonoBehaviour {

	public int ownedByTeamNumber;
	public SoundEffect OpenSound;
	public SoundEffect CloseSound;

	private GameObject _portcullis;

	// Use this for initialization
	void Start () {
		_portcullis = transform.parent.FindChild("Portcullis").gameObject;
		OpenSound = DadaAudio.GetSoundEffect(OpenSound);
		CloseSound = DadaAudio.GetSoundEffect(CloseSound);

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

		if (hero == null )
			return;

		if (hero.PlayerInstance.InTeam.Number + 1 == ownedByTeamNumber && _portcullis.activeSelf == true) {
			_portcullis.SetActive(false);
			OpenSound.PlayEffect();
		}

	}

	void OnTriggerExit2D(Collider2D col){
		Hero hero = col.gameObject.GetComponent<Hero>();
	
		if (hero == null)
			return;
	
		if (hero.PlayerInstance.InTeam.Number + 1 == ownedByTeamNumber && _portcullis.activeSelf == false)
			_portcullis.SetActive(true);
			CloseSound.PlayEffect();
	
	}




}
