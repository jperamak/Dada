using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Dada.InputSystem;

public class CharacterSelectionView : MonoBehaviour {

	public PlayerReady OnPlayerReady;
	public Transform SpawnPoint;

	private GameObject _coverPanel;
	private int _iHero, _iMelee, _iRange, _iTeam;
	private int _btnSelected;

	private AbstractController _controller;
	private Player _player; 
	private Hero _currentHero;
	private Image[] _buttons;
	private Text[] _btnText;
	private bool confirmed = false;

	//find references to children UI elements and inisialize indexes to default  
	void Awake () {
		//_coverPanel = transform.FindChild("Cover").gameObject;
		_iHero = 0;
		_iMelee = 0;
		_iRange = 0;
        _iTeam = 0;
		_btnSelected = 0;
		UpdateBtnSelection();
		//_buttons[_btnSelected].Select();
	}

	//assign a controller to this view. Create a persistent instance of the player
	public void SetController(AbstractController c){
		_controller = c;
		_player = new Player(c);
		AssemblePlayer();
	
		//Color the screen panel with the player's color
		Image background = GetComponent<Image>();
		Color newColor = _player.TeamColor;
		newColor.a = 0.5f;
		background.color = newColor;
	}

	private void AssemblePlayer(){



		if(_currentHero != null)
			Destroy(_currentHero.gameObject);

		//save user's current selection into player class
		_player.Hero = Resource.Heroes[_iHero]; 
		_player.FirstWeapon = Resource.RangedWepons[_iRange];
		_player.SecondWeapon = Resource.MeleeWepons[_iMelee];
        _player.TeamNumber = _iTeam;

		//update UI text to show the current selection
        UpdateBtnText();

		//create an instance of the hero so the player can try it out in the selection screen
		Hero hero = (Instantiate(_player.Hero.Prefab) as GameObject).GetComponent<Hero>();
		Weapon ranged = (Instantiate(_player.FirstWeapon.Prefab) as GameObject).GetComponent<Weapon>();
		Weapon melee = (Instantiate(_player.SecondWeapon.Prefab) as GameObject).GetComponent<Weapon>();

		//hero cannot take damage in selection screen
		Destroy(hero.GetComponent<Damageable>());

		//put the weapons into hero's hands
		ranged.transform.parent = hero.transform.FindChild("Hand1");
		melee.transform.parent = hero.transform.FindChild("Hand2");

		//weapons need to know their owner gameobject (the hero, not the player!)
		ranged.SetOwner(hero.gameObject);
		melee.SetOwner(hero.gameObject);

		//place the hero in the test cage
		hero.transform.localPosition = SpawnPoint.position;
		ranged.transform.localPosition = Vector2.zero;
		melee.transform.localPosition = Vector2.zero;

		_currentHero = hero;
	}

	private void TryPlayer(){}
	private void ConfirmPlayer(){}

	void Update () {

		//waiting that a controller is assigned, or the player is ready to play
		if(_controller == null || confirmed)
			return;

		//Player confirms his selection. Set it as ready
		if(_controller.GetButtonDown(VirtualKey.START)){
			confirmed = true;

			//lock the hero and let the player use it while all the other players are ready
			_currentHero.SetPlayer(_player);
			OnPlayerReady(_player);
			return;
		}
	
		//moving selection up/down: browse buttons
		if(_controller.GetButtonDown(VirtualKey.UP) && _btnSelected > 0){
			_btnSelected--;
			UpdateBtnSelection();
		}
		else if(_controller.GetButtonDown(VirtualKey.DOWN) && _btnSelected < 3){
			_btnSelected++;
			UpdateBtnSelection();
		}

		//change selection
		else if(_controller.GetButtonDown(VirtualKey.LEFT)){

			//hero changed
			if(_btnSelected == 0 && _iHero > 0){
				_iHero--;
				AssemblePlayer();
			}
			//ranged weapon changed
			else if(_btnSelected == 1 && _iRange > 0){
				_iRange--;
				AssemblePlayer();
			}
			//melee weapon changed
			else if(_btnSelected == 2 && _iMelee > 0){
				_iMelee--;
				AssemblePlayer();
			}
            else if (_btnSelected == 3 && _iTeam > 0){
                _iTeam--;
                AssemblePlayer();
            }
		}

		//change selection. Same thing as before, but 
		else if(_controller.GetButtonDown(VirtualKey.RIGHT)){
			if(_btnSelected == 0 && _iHero < Resource.Heroes.Length-1){
				_iHero++;
				AssemblePlayer();
			}
			else if(_btnSelected == 1 && _iRange < Resource.RangedWepons.Length-1){
				_iRange++;
				AssemblePlayer();
			}
			else if(_btnSelected == 2 && _iMelee < Resource.MeleeWepons.Length-1){
				_iMelee++;
				AssemblePlayer();
			}
            else if (_btnSelected == 3 && _iTeam < Resource.Teams.Length - 1){
                _iTeam++;
                AssemblePlayer();
            }
		}
	}

	private void UpdateBtnSelection(){

		if(_buttons == null){
			Button[] btn = transform.GetComponentsInChildren<Button>(); 
			_buttons = new Image[btn.Length];

			for(int i=0;i<btn.Length;i++)
				_buttons[i] = btn[i].GetComponent<Image>();
		}

		for(int i=0;i<_buttons.Length;i++)
			_buttons[i].enabled = false;

		_buttons[_btnSelected].enabled = true;


	}

	private void UpdateBtnText(){

		//if not initialized, cache the text components of the UI buttons into an array
		if(_btnText == null){
			_btnText = new Text[_buttons.Length];
			for(int i=0;i<_buttons.Length;i++)
				_btnText[i] = _buttons[i].transform.GetChild(0).GetComponent<Text>();
		}

		//set the new text for the buttons to show the current selection
		_btnText[0].text = Resource.Heroes[_iHero].Name;
		_btnText[1].text = Resource.RangedWepons[_iRange].Name;
		_btnText[2].text = Resource.MeleeWepons[_iMelee].Name;
        _btnText[3].text = Resource.Teams[_iTeam].Name;

        Image background = GetComponent<Image>();
        Color newColor = _player.TeamColor;
        newColor.a = 0.5f;
        background.color = newColor;

	}
}
