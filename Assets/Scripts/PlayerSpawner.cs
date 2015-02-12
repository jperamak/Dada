using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour {


    public int playerNumber;
    public GameObject playerPrefab;

	void Start()
    {
        var pl = Instantiate(playerPrefab,transform.position, new Quaternion()) as PlayerControl;
        pl.playerNumber = playerNumber;
    }
	
    void Respawn(int number)
    {
        
    }



}
