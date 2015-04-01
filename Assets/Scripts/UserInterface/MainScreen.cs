using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainScreen : MonoBehaviour {

	public RectTransform Layout;

	void Awake(){

		Button[] _buttons = Layout.transform.GetComponentsInChildren<Button>();

		for(int i=0; i<_buttons.Length; i++){
			if(i >= DadaInput.ConrtollerCount)
				_buttons[i].interactable = false;
			if(i==0)
				_buttons[i].Select();
		}

	}

	public void RegisterPlayerNumber(int num){

		//be sure that 
		DadaGame.PlayersNum = Mathf.Clamp(num,0,DadaInput.ConrtollerCount);
		Application.LoadLevel("CharacterSelection");
	}

}
