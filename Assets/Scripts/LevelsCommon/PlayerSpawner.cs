using UnityEngine;
using Dada.InputSystem;
using System.Collections.Generic;

public class PlayerSpawner : MonoBehaviour {

	public GameObject PlayerPrefab;
	public Transform[] SpawnPoints;
    public Sprite[] PlayerSprites;
	
	public GameObject[] RandomSpawn;
	public float RandomSpawnInterval = 20.0f;


	private PlayerHealth[] _assignedHealth;
    private int[] _scores;
	private int _joinedPlayers = 0;
	private float _lastRandom = 0;

	void Start () {
		_assignedHealth = new PlayerHealth[DadaInput.ConrtollerCount];
        _scores = new int[4] { 0,0,0,0};
	}

	void Update () {
		if(_joinedPlayers != DadaInput.ConrtollerCount)
        {
            ConnectPlayers();
        }
        CheckPlayerHealths();
		SpawnThings();
	}

    void CheckPlayerHealths()
    {
        for (int i = 0; i < _assignedHealth.Length; i++)
        {
            if (_assignedHealth[i] != null && _assignedHealth[i].health <= 0)
            {
                _assignedHealth[i] = null;
                _joinedPlayers--;
            }
        }
    }

    void ConnectPlayers()
    {
		AbstractController contr = DadaInput.DetectKeypress(VirtualKey.START);
		if(contr != null)
			Debug.Log("Press start controller "+contr.Number+" "+contr.Name);

		if(contr != null && _assignedHealth[contr.Number] == null){
			_joinedPlayers++;
			GameObject newPlayer = Instantiate(PlayerPrefab) as GameObject;
			newPlayer.GetComponent<PlayerControl>().controller = contr;
			_assignedHealth[contr.Number] = newPlayer.GetComponent<PlayerHealth>();

			//choose random spawn point
			int random = Random.Range(0,SpawnPoints.Length);
			newPlayer.transform.position = SpawnPoints[random].position;
            
            SpriteRenderer[] r = newPlayer.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in r)
            {
                if (sr.gameObject.name == "body")
                    sr.sprite = PlayerSprites[contr.Number];
            }
		}
    }

	private void SpawnThings(){
		if(Time.time > _lastRandom + RandomSpawnInterval){
			_lastRandom = Time.time;
			GameObject randThing = RandomSpawn[Random.Range(0,RandomSpawn.Length)];
			Transform randPoint = SpawnPoints[Random.Range(0,SpawnPoints.Length)];

			GameObject thingInst = Instantiate(randThing,randPoint.position, randPoint.rotation) as GameObject;
			thingInst.transform.localScale = new Vector3(0.5f,0.5f,1);

		}

	}

    public void AddPoint(int playerNum, int amount)
    {
        _scores[playerNum] += amount;
        GameObject.Find("Scores" + playerNum).GetComponent<GUIText>().text = ""+_scores[playerNum];
    }
}
