using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainScreen : MonoBehaviour {

	public void RegisterPlayerNumber(int num){
		DadaGame.PlayersNum = num;
		Application.LoadLevel("CharacterSelection");
	}

}
