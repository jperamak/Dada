using UnityEngine;
using System.Collections;

/* This class follows the Typesafe Enum design pattern */
public sealed class Level {
	
	
	public static readonly Level MainScreen = new Level("Main Screen", "MainScreen");
	public static readonly Level Croco = new Level("Crock City", "RikusSandbox");
	public static readonly Level Planets = new Level("Planets", "Antigravity");
	
	public string Name{get; private set;}
	private string _realName;

	public void Load(){
		Application.LoadLevel(_realName);
	}

	private Level(string name, string systemName){
		Name = name;
		_realName = systemName;
	}
	
	public static Level[] Levels = { Level.MainScreen, Level.Croco, Level.Planets };
}
