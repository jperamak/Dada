using UnityEngine;
using Dada.InputSystem;
using System.Collections.Generic;

public class PlayerSpawner : MonoBehaviour {

	public GameObject PlayerPrefab;
	public Transform[] SpawnPoints;

	private PlayerHealth[] _assignedHealth;
    private int[] _scores;
	private int _joinedPlayers = 0;

	void Start () {
		_assignedHealth = new PlayerHealth[DadaInput.ConrtollerCount];
		Debug.Log("There are "+DadaInput.ConrtollerCount);
        _scores = new int[4] { 0,0,0,0};
	}

	void Update () {
		if(_joinedPlayers != DadaInput.ConrtollerCount)
        {
            ConnectPlayers();
        }
        CheckPlayerHealths();
	}

    void CheckPlayerHealths()
    {
        for (int i = 0; i < _assignedHealth.Length; i++)
        {
            if (_assignedHealth[i] != null && _assignedHealth[i].health <= 0)
            {
                _assignedHealth[i] = null;
                _joinedPlayers--;
                Debug.Log(i + " died");
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
			//if(SpawnPoints.Length < _joinedPlayers){
				int random = Random.Range(0,SpawnPoints.Length);
				newPlayer.transform.position = SpawnPoints[random].position;
			//}
			//else
            //{
			//	newPlayer.transform.position = SpawnPoints[_joinedPlayers-1].position;
			//}
		}
    }

    public void AddPoint(int playerNum, int amount)
    {
        _scores[playerNum] += amount;
        GameObject.Find("Scores" + playerNum).GetComponent<GUIText>().text = ""+_scores[playerNum];
    }
}
