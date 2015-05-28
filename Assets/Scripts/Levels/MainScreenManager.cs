using UnityEngine;
using UnityEngine.UI;
using Dada.InputSystem;
using System.Collections.Generic;

public class MainScreenManager : MonoBehaviour {

	public enum Screen{START, CREDITS, PLAYER_SELECTION, LEVEL_SELECTION, MODE_SELECTION, BLACKSCREEN};

	public Text StartGameText;
	public Text CreditsText;
	public Text TeamModeText;
	public Text AllVsAllModeText;
	
	public FadeEffect CreditsPage;
	public FadeEffect StartPage;
	public FadeEffect ModePage;
	public FadeEffect BlackScreen;

	public Animator LogoAnimator;
	public Animator EggsAnimator;
	public Animator[] PlayerEggsAnim;
	public Animator[] CowLevels;
	public string[] LevelsName;

	public SoundEffect MenuBackAudio;
	public SoundEffect MenuConfirmAudio;
	public SoundEffect MenuBipAudio;
	public SoundEffect MenuConfirmEggsAudio;

	private FadeEffect _startFadeText;
	private FadeEffect _creditsFadeText;
	private FadeEffect _teamFadeText;
	private FadeEffect _allvsFadeText;

	private Screen _currentScreen;
	private AbstractController _controller;
	private int _playerNum = 0;
	private int _levelNum = 0;
	private string _levelName;
	private bool _isTeamMode = true;



	// Use this for initialization
	void Start () { 

		_startFadeText = StartGameText.GetComponent<FadeEffect>();
		_creditsFadeText = CreditsText.GetComponent<FadeEffect>();
		_teamFadeText = TeamModeText.GetComponent<FadeEffect>();
		_allvsFadeText = AllVsAllModeText.GetComponent<FadeEffect>();

		_startFadeText.Fade();
		StartGameText.color = Color.yellow;

		_controller = DadaInput.GetJoystick(0);
		_currentScreen = Screen.START;


	}
	
	// Update is called once per frame
	void Update () {

		if(_currentScreen == Screen.START){

			//highlight Start text
			if(_controller.GetButtonDown(VirtualKey.UP) && !_startFadeText.IsFading){
				_startFadeText.Fade();
				_creditsFadeText.Stop(1,0);
				StartGameText.color = Color.yellow;
				CreditsText.color = Color.white;
				MenuBipAudio.PlayEffect();
			}

			//highlight Credits text
			if(_controller.GetButtonDown(VirtualKey.DOWN) && !_creditsFadeText.IsFading){
				_creditsFadeText.Fade();
				_startFadeText.Stop(1,0);
				CreditsText.color = Color.yellow;
				StartGameText.color = Color.white;
				MenuBipAudio.PlayEffect();
			}

			//player pressed submit
			if(_controller.GetButtonDown(VirtualKey.SUBMIT)){

				//Go to credits
				if(_creditsFadeText.IsFading){
					_creditsFadeText.Stop(1,0);
					CreditsText.color = Color.white;
					LogoAnimator.SetTrigger("Reduce");
					StartPage.Fade(1,0,0,0.3f);
					CreditsPage.Fade(0,1,0.3f, 0.3f);
					_currentScreen = Screen.CREDITS;
				}

				//Go to player selection
				else{
					_creditsFadeText.Stop(1,0);
					CreditsText.color = Color.white;
					StartPage.Fade(1,0,0,0.3f);
					EggsAnimator.SetTrigger("PopIn");
					_currentScreen = Screen.PLAYER_SELECTION;
				}

				MenuConfirmAudio.PlayEffect();
			}
		}
		else if(_currentScreen == Screen.CREDITS){

			//player pressed back
			if(_controller.GetButtonDown(VirtualKey.BACK)){
				
				//Go to start
				_startFadeText.Fade();
				StartGameText.color = Color.yellow;
				LogoAnimator.SetTrigger("Enlarge");
				CreditsPage.Fade(1,0,0,0.3f);
				StartPage.Fade(0,1,0.3f, 0.3f);
				_currentScreen = Screen.START;
				MenuBackAudio.PlayEffect();

			}
		}
		else if(_currentScreen == Screen.PLAYER_SELECTION){

			//Go to Start and close eggs
			if(_controller.GetButtonDown(VirtualKey.BACK)){
				_startFadeText.Fade();
				StartGameText.color = Color.yellow;

				for(int i = _playerNum-1; i >= 0; i--)
					PlayerEggsAnim[i].SetTrigger("Close");

				EggsAnimator.SetTrigger("PopOut");
				StartPage.Fade(0,1,1f, 0.3f);
				_currentScreen = Screen.START;
				_playerNum = 0;
				MenuBackAudio.PlayEffect();
			}

			//open new egg
			if(_controller.GetButtonDown(VirtualKey.RIGHT)){
				if(_playerNum < DadaInput.ConrtollerCount){
					PlayerEggsAnim[_playerNum].SetTrigger("Crack");
					_playerNum++;
					MenuBipAudio.PlayEffect();
				}

			}

			//close egg
			if(_controller.GetButtonDown(VirtualKey.LEFT)){
				if(_playerNum > 0){
					_playerNum--;
					PlayerEggsAnim[_playerNum].SetTrigger("Close");
					MenuBipAudio.PlayEffect();
				}
			}

			//Smash eggs and show Levels
			if(_controller.GetButtonDown(VirtualKey.SUBMIT)){

				LogoAnimator.SetTrigger("Reduce");
				EggsAnimator.SetTrigger("Smash");

				for(int i=0; i< CowLevels.Length; i++){
					CowLevels[i].SetBool("IsDancing",false);
					CowLevels[i].SetTrigger("Appear");

					CowLevels[i].transform.parent.gameObject.SetActive(true);
				}

				CowLevels[0].SetBool("IsDancing",true);
				MenuConfirmAudio.PlayEffect();
				_currentScreen = Screen.LEVEL_SELECTION;
			}
		}
		else if(_currentScreen == Screen.LEVEL_SELECTION){

			//Go to Start and close eggs
			if(_controller.GetButtonDown(VirtualKey.BACK)){
				MenuBackAudio.PlayEffect();
				LogoAnimator.SetTrigger("Enlarge");
				EggsAnimator.SetTrigger("UnSmash");

				for(int i=0; i< CowLevels.Length; i++){
					CowLevels[i].SetTrigger("Disappear");
				}

				for(int i = 0; i < _playerNum; i++)
					PlayerEggsAnim[i].SetTrigger("Crack");

				_levelNum = 0;
				_currentScreen = Screen.PLAYER_SELECTION;
			}

			//make next cowlevel dance
			if(_controller.GetButtonDown(VirtualKey.RIGHT)){
				if(_levelNum+1 < CowLevels.Length){
					CowLevels[_levelNum].SetBool("IsDancing",false);
					CowLevels[_levelNum+1].SetBool("IsDancing",true);
					MenuBipAudio.PlayEffect();
					_levelNum++;
				}
			}
			
			///make previous cowlevel dance
			if(_controller.GetButtonDown(VirtualKey.LEFT)){
				if(_levelNum > 0){
					CowLevels[_levelNum].SetBool("IsDancing",false);
					CowLevels[_levelNum-1].SetBool("IsDancing",true);
					MenuBipAudio.PlayEffect();
					_levelNum--;
				}
			}

			//go to mode selection
			if(_controller.GetButtonDown(VirtualKey.SUBMIT)){

				for(int i=0; i< CowLevels.Length; i++){
					if(i != _levelNum)
						CowLevels[i].SetTrigger("Disappear");
				}

				//Go to start
				_teamFadeText.Fade();
				_allvsFadeText.Stop(1,0);
				TeamModeText.color = Color.yellow;
				AllVsAllModeText.color = Color.white;
				ModePage.Fade(0,1,0.3f, 0.3f);
				_isTeamMode = true;
				MenuConfirmAudio.PlayEffect();
				_currentScreen = Screen.MODE_SELECTION;

			}
		}
		else if(_currentScreen == Screen.MODE_SELECTION){

			//highlight team mode text
			if(_controller.GetButtonDown(VirtualKey.LEFT) && !_teamFadeText.IsFading){
				_teamFadeText.Fade();
				_allvsFadeText.Stop(1,0);
				TeamModeText.color = Color.yellow;
				AllVsAllModeText.color = Color.white;
				_isTeamMode = true;
				MenuBipAudio.PlayEffect();
			}
			
			//highlight Credits text
			if(_controller.GetButtonDown(VirtualKey.RIGHT) && !_creditsFadeText.IsFading){
				_allvsFadeText.Fade();
				_teamFadeText.Stop(1,0);
				AllVsAllModeText.color = Color.yellow;
				TeamModeText.color = Color.white;
				_isTeamMode = false;
				MenuBipAudio.PlayEffect();
			}

			if(_controller.GetButtonDown(VirtualKey.BACK)){
				MenuBackAudio.PlayEffect();
				ModePage.Fade(1,0,0,0.3f);

				for(int i=0; i< CowLevels.Length; i++){
					if(i != _levelNum)
						CowLevels[i].SetTrigger("Appear");
				}
				_currentScreen = Screen.LEVEL_SELECTION;
			}

			if(_controller.GetButtonDown(VirtualKey.SUBMIT)){
				BlackScreen.Fade();
				MenuConfirmAudio.PlayEffect();
				Invoke("BeginGame",0.6f);
				_currentScreen = Screen.BLACKSCREEN;
			}
		}
	}

	private void BeginGame(){

		_levelName = LevelsName[_levelNum];

		DadaGame.PlayersNum = Mathf.Clamp(_playerNum,0,DadaInput.ConrtollerCount);
		DadaGame.IsTeamPlay = _isTeamMode;
		
		if(DadaGame.PlayersNum >= 1){
			Player p = new Player(DadaInput.GetJoystick(0));
			p.Hero = Resource.FISH_HERO;
			p.FirstWeapon = Resource.PHOENIX;
			p.SecondWeapon = Resource.LAYBOMB_MELEE;
			p.InTeam = Team.TEAM_1;
			DadaGame.RegisterPlayer(p);
		}
		
		if(DadaGame.PlayersNum >= 2){
			Player p = new Player(DadaInput.GetJoystick(1));
			p.Hero = Resource.FEZ_HERO;
			p.FirstWeapon = Resource.PHOENIX;
			p.SecondWeapon = Resource.LAYBOMB_MELEE;
			p.InTeam = Team.TEAM_2;
			DadaGame.RegisterPlayer(p);
		}
		
		if(DadaGame.PlayersNum >= 3){
			Player p = new Player(DadaInput.GetJoystick(2));
			p.Hero = Resource.RACCOON_HERO;
			p.FirstWeapon = Resource.PHOENIX;
			p.SecondWeapon = Resource.LAYBOMB_MELEE;
			p.InTeam = _isTeamMode ? Team.TEAM_1 : Team.TEAM_3;
			DadaGame.RegisterPlayer(p);
		}
		
		if(DadaGame.PlayersNum >= 4){
			Player p = new Player(DadaInput.GetJoystick(3));
			p.Hero = Resource.POOP_HERO;
			p.FirstWeapon = Resource.PHOENIX;
			p.SecondWeapon = Resource.LAYBOMB_MELEE;
			p.InTeam = _isTeamMode ? Team.TEAM_2 : Team.TEAM_4;
			DadaGame.RegisterPlayer(p);
		}

		Application.LoadLevel(_levelName);
		
	}

}
