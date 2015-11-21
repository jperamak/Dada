using UnityEngine;
using System.Collections.Generic;

public class CharacterSelectionScreen : MonoBehaviour {

	private int _playersReady = 0;

	// Use this for initialization
	void Start () {
		Canvas canvas = GameObject.FindObjectOfType<Canvas>();

		CharacterSelectionView[] views = new CharacterSelectionView[4];

		//If you are debugging and you start directly from the character selection screen, 
		//this gives you at least one player to play with
		if(DadaGame.PlayersNum == 0)
			DadaGame.PlayersNum = 1;

		//Assign the controllers to specific sub-windows, so players can choose their characer indipendently.
		//Disable windows without a player
		for(int i=0;i<4;i++){
			views[i] = canvas.transform.GetChild(i).GetComponent<CharacterSelectionView>();
			if(i < DadaGame.PlayersNum && i < DadaInput.ControllerCount){
				views[i].OnPlayerReady = PlayerReady;	
				views[i].SetController(DadaInput.GetJoystick(i));
			}
			else
				views[i].gameObject.SetActive(false);
		}
	}

	//FIXME!! THIS IS DEBUG ONLY. It should go to level selection screen
	//When all the players are ready, load the level
	private void PlayerReady(Player player){
		_playersReady ++;

		DadaGame.RegisterPlayer(player);
		if(_playersReady == DadaGame.PlayersNum)
			Application.LoadLevel("LevelSelection");
	}
	

}
