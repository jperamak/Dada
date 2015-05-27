using UnityEngine;
using System.Collections;

/* This class follows the Typesafe Enum design pattern */
public sealed class Team {

	public enum TeamID {TEAM_1, TEAM_2, TEAM_3, TEAM_4};

	public static readonly Team TEAM_1 = new Team("The Bastards", Color.red,0, TeamID.TEAM_1);
	public static readonly Team TEAM_2 = new Team("team_name", new Color(18f/255f,74f/255f,214f/255f),1, TeamID.TEAM_2);
	public static readonly Team TEAM_3 = new Team("Curry Soup", new Color(35f/255f,176f/255f,45f/255f),2, TeamID.TEAM_3);
	public static readonly Team TEAM_4 = new Team("Dolly Blues", Color.yellow,3, TeamID.TEAM_4);
	
	public string Name{get; private set;}
	public Color TeamColor{get; private set;}
	public int Number{get; private set;}
	public TeamID Id{get; private set;}

	private Team(string name, Color c, int n, TeamID id){
		Name = name;
		TeamColor = c;
		Number = n;
		Id = id;
	}

	public static Team[] Teams = { Team.TEAM_1, Team.TEAM_2, Team.TEAM_3, Team.TEAM_4 };
}
