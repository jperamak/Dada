using UnityEngine;
using Dada.InputSystem;
using System.Collections;


public class Player{

	public int Number{get; private set;}
    private int _teamNumber;
    public int TeamNumber
    {
        get { return _teamNumber; }
        set
        {
            _teamNumber = value;
            if (_teamNumber == 0)
                TeamColor = Color.red;
            else if (_teamNumber == 1)
                TeamColor = Color.blue;
            else if (_teamNumber == 2)
                TeamColor = Color.green;
            else if (_teamNumber == 3)
                TeamColor = Color.yellow;
        }
    }

    public Color TeamColor { get; private set; }
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
	}


	
}
