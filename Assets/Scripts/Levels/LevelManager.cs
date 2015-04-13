using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

	public float RespawnTime = 2.0f;
	public SoundEffect RespawnSound;
    public int MaxScore = 10;
	public static LevelManager Current{get; private set;}
	
	protected SpawnPoint[] _spawnPoints;
	protected int[] _scores;
	protected List<Team> _teams;
	protected List<Text> _scoreText; 

    private CameraFollow _camera;
	private Text _fin;
	private Transform _pauseScreen;
	private bool _isPaused = false;


	protected void Awake(){
		Current = this;
        RespawnSound = DadaAudio.GetSoundEffect(RespawnSound);
	}

	protected void Start(){
		Transform canvas = GameObject.Find("Canvas").transform;
		Transform scoreText = canvas.Find("Scores");
		_fin = canvas.transform.FindChild("Fin").GetComponent<Text>();
		_pauseScreen = canvas.transform.FindChild("PauseScreen");
        _camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>();

		_teams = DadaGame.Teams;

		//********** FOR DEBUG ONLY!! **************
		if(_teams == null){
			DadaGame.RegisterPlayer(CreateDebugPlayers());
			_teams = DadaGame.Teams;
		}

		//find all respawn points
		_spawnPoints = Object.FindObjectsOfType<SpawnPoint>();

		//shuffle the order of spawnpoints, so the players will spawn to different places
		ShuffleSpawnPoints();

		//find the scores UI and give them the same color of the player
		_scoreText = new List<Text>();
		for(int i=0; i<scoreText.childCount; i++){

			//there is no team for this score: hide the text
			if(i >= _teams.Count)
				scoreText.GetChild(i).gameObject.SetActive(false);
			else{
				_scoreText.Add(scoreText.GetChild(i).GetComponent<Text>());
				_scoreText[i].color = _teams[i].TeamColor;
			}

		}

		InitLevel();
	}
    
	void Update(){
		bool startPressed = DadaInput.GetButtonDown(VirtualKey.START);


		//Pause game
		if(!_isPaused && startPressed){
			Time.timeScale = 0.0000001f;
			_isPaused = true;
			_pauseScreen.gameObject.SetActive(true);
		}
		else if(_isPaused){
			//resume game
			if(startPressed){
				Time.timeScale = 1;
				_isPaused = false;
				_pauseScreen.gameObject.SetActive(false);
			}
			else if(DadaInput.GetButtonDown(VirtualKey.SELECT)){
				Time.timeScale = 1;
				_isPaused = false;
				DadaGame.Reset();
				Application.LoadLevel("MainScreen");
			}
		}
	}

    public virtual void InitLevel(){

		_scores = new int[DadaGame.TeamsNum];

		UpdateScore();

		for(int i=0; i<_scores.Length; i++)
			_scores[i] = 0;
	
		List<Player> players = DadaGame.Players;
		for(int i=0; i<players.Count; i++){
			SpawnHero(players[i]);
		}
	}

	public virtual void SpawnHero(Player p){

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
		SpawnPoint randomSpawn = null;
		bool found = false;

		//pick only certain spawnpoints for teams
		//if(DadaGame.IsTeamPlay){
			for(int i=0; i<_spawnPoints.Length && !found; i++){
				if(_spawnPoints[i].IsValidForTeam(p.InTeam.Number)){
					randomSpawn = _spawnPoints[i];
					found=true;
				}
			}
			ShuffleSpawnPoints();


		//every spawnpoint is ok for non teamplay
		/*else
			randomSpawn = _spawnPoints[Random.Range(0,_spawnPoints.Length)];
*/
		hero.transform.position = randomSpawn.transform.position;

		if(RespawnSound != null)
			RespawnSound.PlayEffect();

        _camera.AddPlayer(hero.transform);
	}

	protected virtual IEnumerator RespawnCountdown(Player p){
		yield return new WaitForSeconds(RespawnTime);
		SpawnHero(p);
	}

	protected void OnPlayerKilled(GameObject victim, GameObject killer = null){


		Player v = victim.GetComponent<Hero>().PlayerInstance;
		Player k = null;
		if(killer != null)
        	k = killer.GetComponent<Hero>().PlayerInstance;

		//player suicided or killed for something else
        if (FriendlyFire(v, k)){
            _scores[_teams.IndexOf(k.InTeam)]--;
			_scores[_teams.IndexOf(v.InTeam)]--;
        }
        else if(killer == null || victim == killer ){
			_scores[_teams.IndexOf(v.InTeam)]--;
		}

		//player killed by another player
		else{
			//Player k = killer.GetComponent<Hero>().PlayerInstance;
			_scores[_teams.IndexOf(k.InTeam)]++;
		}

		UpdateScore();
        _camera.RemovePlayer(victim.transform);
		StartCoroutine(RespawnCountdown(v));
	}

    private bool FriendlyFire(Player victim, Player killer)
    {
        if (victim != null && killer != null)
            if ( victim != killer)
                return victim.InTeam.Number == killer.InTeam.Number;
        return false;
    }

	private void UpdateScore(){

		for(int i=0; i< DadaGame.Teams.Count; i++)
			_scoreText[i].text = _teams[i].Name+": "+_scores[i];
		
		for (int i = 0; i < DadaGame.Teams.Count; i++)
			if (_scores[i] >= MaxScore)
				Finish(i);
	}

    private void Finish(int winner)
    {
        Time.timeScale = 0.5f;

        
        _fin.text = "Team " + _teams[winner].Name + " wins!";
		_fin.color = _teams[winner].TeamColor;
        _fin.transform.gameObject.SetActive(true);
        Invoke("NextLevel",2);
    }

    private void NextLevel(){
        Time.timeScale = 1f;
        Application.LoadLevel(Application.loadedLevelName);
    }

	private void ShuffleSpawnPoints(){
		for(int i=0; i<_spawnPoints.Length; i++){
			int randIndex = Random.Range(0,_spawnPoints.Length);
			SpawnPoint t = _spawnPoints[i];
			_spawnPoints[i] = _spawnPoints[randIndex];
			_spawnPoints[randIndex] = t;
		}
	}

	private Player CreateDebugPlayers(){

		Player p1 = new Player(DadaInput.GetJoystick(0));
		p1.Hero = Resource.MONK_HERO;
		p1.FirstWeapon = Resource.PHOENIX;
		p1.SecondWeapon = Resource.LAYBOMB_MELEE;
		p1.InTeam = Team.TEAM_1;
		return p1;
	}

}
