using UnityEngine;
using System.Collections;

/* This class follows the Typesafe Enum design pattern */
public sealed class Team {
	

	public static readonly Team TEAM_1 = new Team("The Bastards", Color.red,0);
	public static readonly Team TEAM_2 = new Team("team_name", Color.blue,1);
	public static readonly Team TEAM_3 = new Team("Curry Soup", Color.green,2);
	public static readonly Team TEAM_4 = new Team("Dolly Blues", Color.yellow,3);
	
	public string Name{get; private set;}
	public Color TeamColor{get; private set;}
	public int Number{get; private set;}

	private Team(string name, Color c, int n){
		Name = name;
		TeamColor = c;
		Number = n;
	}

	public static Team[] Teams = { Team.TEAM_1, Team.TEAM_2, Team.TEAM_3, Team.TEAM_4 };
}
