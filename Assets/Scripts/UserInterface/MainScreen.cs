using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainScreen : MonoBehaviour {

	public RectTransform ButtonLayout;
	public RectTransform TitleScreen;
	public RectTransform LevelScreen;
	public Button DefaultLevelButton;

	private Button _selectedDefaultPlayers;

	void Awake(){

		TitleScreen.gameObject.SetActive(true);
		LevelScreen.gameObject.SetActive(false);

		Button[] _buttons = ButtonLayout.transform.GetComponentsInChildren<Button>();

		for(int i=0; i<_buttons.Length; i++){
			if(i >= DadaInput.ConrtollerCount)
				_buttons[i].interactable = false;
			if(i==0){
				_selectedDefaultPlayers = _buttons[i];
				_selectedDefaultPlayers.Select();
			}
		}
	}

	void Update(){
		if(LevelScreen.gameObject.activeSelf){
			if(DadaInput.GetJoystick(0).GetButtonDown(VirtualKey.BACK))
				ShowTitle();
		} 
	}

	public void LoadLevelTeam(string level){

		SetupPlayers(true);
		Application.LoadLevel(level);
	}

	public void LoadLevel(string level){

		SetupPlayers(false);
		Application.LoadLevel(level);
	}

	private void ShowLevels(){
		TitleScreen.gameObject.SetActive(false);
		LevelScreen.gameObject.SetActive(true);
		DefaultLevelButton.Select();
	}

	public void ShowTitle(){
		TitleScreen.gameObject.SetActive(true);
		LevelScreen.gameObject.SetActive(false);
		_selectedDefaultPlayers.Select();
	}

	private void SetupPlayers(bool isTeam){

		if(DadaGame.PlayersNum >= 1){
			Player p = new Player(DadaInput.GetJoystick(0));
			p.Hero = Resource.POT_HERO;
			p.FirstWeapon = Resource.PHOENIX;
			p.SecondWeapon = Resource.LAYBOMB_MELEE;
			p.InTeam = Team.TEAM_1;
			DadaGame.RegisterPlayer(p);
		}
		
		if(DadaGame.PlayersNum >= 2){
			Player p = new Player(DadaInput.GetJoystick(1));
			p.Hero = Resource.FISH_HERO;
			p.FirstWeapon = Resource.PHOENIX;
			p.SecondWeapon = Resource.LAYBOMB_MELEE;
			p.InTeam = Team.TEAM_2;
			DadaGame.RegisterPlayer(p);
		}
		
		if(DadaGame.PlayersNum >= 3){
			Player p = new Player(DadaInput.GetJoystick(2));
			p.Hero = Resource.CAT_HERO;
			p.FirstWeapon = Resource.PHOENIX;
			p.SecondWeapon = Resource.LAYBOMB_MELEE;
			p.InTeam = isTeam ? Team.TEAM_1 : Team.TEAM_3;
			DadaGame.RegisterPlayer(p);
		}
		
		if(DadaGame.PlayersNum >= 4){
			Player p = new Player(DadaInput.GetJoystick(3));
			p.Hero = Resource.FEZ_HERO;
			p.FirstWeapon = Resource.PHOENIX;
			p.SecondWeapon = Resource.LAYBOMB_MELEE;
			p.InTeam = isTeam ? Team.TEAM_1 : Team.TEAM_4;
			DadaGame.RegisterPlayer(p);
		}

	}

	public void RegisterPlayerNumber(int num){

		//be sure that 
		DadaGame.PlayersNum = Mathf.Clamp(num,0,DadaInput.ConrtollerCount);
		ShowLevels();
	}

}
