using UnityEngine;
using Dada.InputSystem;
using System.Collections;


public class Player{

	public int Number{get; private set;}
	public AbstractController Controller{get; private set;}
    
	public Team InTeam{get; set;}
	public Resource Hero{get; set;}
	public Resource FirstWeapon{get; set;}
	public Resource SecondWeapon{get; set;}


	public Player(AbstractController controller, int playerNum = -1){
		if(controller == null)
			Debug.LogError("Player initialization requires a controller!");

		Controller = controller;
		if(playerNum == -1)
			Number = Controller.Number;
		else
			Number = playerNum;
	}


	
}
