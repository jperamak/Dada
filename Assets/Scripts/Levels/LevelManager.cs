using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

	public float RespawnTime = 2.0f;
	public AudioClip RespawnSound;

	protected List<Player> _players;
	protected List<Transform> _spawnPoints;
	protected int[] _scores;
	protected List<Text> _scoreText; 

	public static LevelManager Current{get; private set;}

	protected void Awake(){
		Current = this;
	}

	protected void Start(){

		//save reference to player array
		_players = DadaGame.Players;

		//********** FOR DEBUG ONLY!! **************
		if(_players == null)
			_players = CreateDebugPlayers();

		//find all respawn points
		GameObject[] spawns = GameObject.FindGameObjectsWithTag("Respawn");

		//shuffle the order of spawnpoints, so the players will spawn to different places
		for(int i=0; i<spawns.Length; i++){
			int randIndex = Random.Range(0,spawns.Length);
			GameObject t = spawns[i];
			spawns[i] = spawns[randIndex];
			spawns[randIndex] = t;
		}

		_spawnPoints = new List<Transform>();
		for(int i=0; i<spawns.Length; i++)
			_spawnPoints.Add(spawns[i].transform); 


		//find the scores UI and give them the same color of the player
		Transform canvas = GameObject.Find("Canvas").transform;
		_scoreText = new List<Text>();
		for(int i=0; i<canvas.childCount; i++){

			//there is no player for this score: hide the text
			if( i >= _players.Count)
				canvas.GetChild(i).gameObject.SetActive(false);
			else{
				_scoreText.Add(canvas.GetChild(i).GetComponent<Text>());
				_scoreText[i].color = _players[i].TeamColor;
			}

		}

		InitLevel();
	}

	public virtual void InitLevel(){

		_scores = new int[_players.Count];

		UpdateScore();

		for(int i=0; i<_players.Count; i++){
			_scores[i] = 0;
			SpawnHero(i);
		}
	}

	public virtual void SpawnHero(int number){
		Player p = _players[number];

		//create an instance of the hero so the player can try it out in the selection screen
		Hero hero = (Instantiate(p.Hero.Prefab) as GameObject).GetComponent<Hero>();
		Weapon ranged = (Instantiate(p.FirstWeapon.Prefab) as GameObject).GetComponent<Weapon>();
		Weapon melee = (Instantiate(p.SecondWeapon.Prefab) as GameObject).GetComponent<Weapon>();
		Damageable dmg = hero.GetComponent<Damageable>();

		dmg.OnDestroy += OnPlayerKilled;

		//put the weapons into hero's hands
		ranged.transform.parent = hero.transform.FindChild("Hand1");
		melee.transform.parent = hero.transform.FindChild("Hand2");
		ranged.transform.localPosition = Vector2.zero;
		melee.transform.localPosition = Vector2.zero;

		//give the soul to the flesh
		hero.SetPlayer(p);

		//weapons need to know their owner gameobject (the hero, not the player!)
		ranged.SetOwner(hero.gameObject);
		melee.SetOwner(hero.gameObject);


		//place the hero in the scene
		int randomSpawn = Random.Range(0, _spawnPoints.Count-1);
		hero.transform.position = _spawnPoints[randomSpawn].position;

		if(RespawnSound != null)
			DadaAudio.PlaySound(RespawnSound);

	}

	protected virtual IEnumerator RespawnCountdown(int playerNumber){
		yield return new WaitForSeconds(RespawnTime);
		SpawnHero(playerNumber);
	}

	protected void OnPlayerKilled(GameObject victim, GameObject killer = null){

		Player v = victim.GetComponent<Hero>().PlayerInstance;

		//player suicided or killed for something else
		if(killer == null || victim == killer){
			_scores[v.Number]--;
		}

		//player killed by another player
		else{
			Player k = killer.GetComponent<Hero>().PlayerInstance;
			_scores[k.Number]++;
		}

		UpdateScore();
		StartCoroutine(RespawnCountdown(v.Number));
	}

	private void UpdateScore(){
		for(int i=0; i< _players.Count; i++)
			_scoreText[i].text = "Player "+_players[i].Number+": "+_scores[i];
	}

	private List<Player> CreateDebugPlayers(){
		List<Player> fake = new List<Player>();

		Player p1 = new Player(DadaInput.GetJoystick(0));
		p1.Hero = Resource.MONK_HERO;
		p1.FirstWeapon = Resource.PHOENIX;
		p1.SecondWeapon = Resource.FLAME_MELEE;

		fake.Add(p1);
		return fake;
	}
	
	public void Pause(){}
	public void UnPause(){}
}
