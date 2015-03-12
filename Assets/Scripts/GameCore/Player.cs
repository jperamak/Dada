using UnityEngine;
using Dada.InputSystem;
using System.Collections;


public class Player{

	public int Number{get; private set;}
	public Color TeamColor{get; private set;}
	public AbstractController Controller{get; private set;}


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

		if(Number == 0)
			TeamColor = Color.red;
		else if(Number == 1)
			TeamColor = Color.blue;
		else if(Number == 2)
			TeamColor = Color.green;
		else if(Number == 3)
			TeamColor = Color.yellow;
	}
	
}
