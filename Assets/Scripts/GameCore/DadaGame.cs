using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class DadaGame {

	public static int PlayersNum{get; set;}
	public static List<Player> Players{get{return _players;}}

	private static List<Player> _players;

	public static void RegisterPlayer(Player player){
		if(_players == null)
			_players = new List<Player>();

		if(_players.Contains(player))
			return;

		_players.Add(player);
		_players.Sort((x, y) => x.Number.CompareTo(y.Number));
	}

	public static void PlaySound(AudioClip clip){
		AudioSource.PlayClipAtPoint(clip, Vector2.zero, 1.0f);
	}

	public static void Reset(){
		_players.Clear();
		_players = null;
		PlayersNum = 0;
	}
}
