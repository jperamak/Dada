using UnityEngine;
using System.Collections;

public class GateKeeper : MonoBehaviour {

	public Team.TeamID OwnedByTeam;
	public SoundEffect OpenSound;
	public SoundEffect CloseSound;

	private GameObject _portcullis;
	private int _playersInArea = 0;

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


	private void OpenGate(){
		if (_playersInArea == 0) {
			_portcullis.SetActive(false);
			if (OpenSound != null)
				OpenSound.PlayEffect();
		}

		_playersInArea++;
	}

	private void CloseGate(){
		_playersInArea--;

		if(_playersInArea == 0){
			_portcullis.SetActive(true);
			if (CloseSound != null)
				CloseSound.PlayEffect();
		}
	}

	//BUG: IF THERE ARE TWO TEAM MEMBERS IN AREA THEN THIS WONT WORK PROPERBLY
	void OnTriggerEnter2D(Collider2D col){
		Hero hero = col.gameObject.GetComponent<Hero>();

		if (hero == null )
			return;

		Team heroTeam = hero.PlayerInstance.InTeam;

		if(!DadaGame.IsTeamPlay || (DadaGame.IsTeamPlay && heroTeam.Id == OwnedByTeam ))
			OpenGate();

	}

	void OnTriggerExit2D(Collider2D col){
		Hero hero = col.gameObject.GetComponent<Hero>();
	
		if (hero == null)
			return;

		Team heroTeam = hero.PlayerInstance.InTeam;
		if(!DadaGame.IsTeamPlay || (DadaGame.IsTeamPlay && heroTeam.Id == OwnedByTeam ))
			CloseGate();
	
	}




}
