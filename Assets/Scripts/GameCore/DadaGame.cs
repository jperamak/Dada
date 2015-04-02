﻿using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DadaGame {

	public static bool IsTeamPlay{get; private set;}
	public static int PlayersNum{get; set;}
	public static int TeamsNum{get{return _teams.Count;}}
	public static List<Player> Players{get{return _players;}}
	public static List<Team> Teams{get{return _teams.Keys.ToList();}}

	private static List<Player> _players;
	private static SortedList<Team,List<Player>> _teams = new SortedList<Team, List<Player>>(new TeamComparer());

	public static void RegisterPlayer(Player player){
		if(_players == null)
			_players = new List<Player>();

		if(_players.Contains(player))
			return;

		_players.Add(player);
		_players.Sort((x, y) => x.Number.CompareTo(y.Number));

		//player's team doesn't exist yet. create it
		if(!_teams.ContainsKey(player.InTeam)){
			_teams.Add(player.InTeam, new List<Player>());
		}

		//enable teams. it means that there are at least 2 players in the same team. 
		else
			IsTeamPlay = true;

		//in every case add the player to its team
		_teams[player.InTeam].Add(player);
	}

	public static List<Player> GetPlayersInTeam(Team team){
		if(_teams.ContainsKey(team))
			return _teams[team];
		return null;
	}

	public static void Reset(){
		_players.Clear();
		_players = null;
		PlayersNum = 0;

		_teams.Clear();
		IsTeamPlay = false;
	}

	/**/
	public class TeamComparer : IComparer<Team>{
		public int Compare(Team a, Team b){
			return ((Team)a).Number.CompareTo(((Team)b).Number);
		}
	}

}
